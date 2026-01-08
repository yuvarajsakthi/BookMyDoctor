using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;
using AutoMapper;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync("Patient,Doctor,Clinic");
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetPatientAppointmentsAsync(int patientId)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(
                a => a.PatientId == patientId,
                "Patient,Doctor,Clinic");
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<Appointment> BookAppointmentAsync(int patientId, BookAppointmentDto request)
        {
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = request.DoctorId,
                ClinicId = request.ClinicId,
                AppointmentDate = request.Date,
                StartTime = request.StartTime,
                EndTime = request.StartTime.AddMinutes(30),
                Status = AppointmentStatus.Pending,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Get doctor and patient details for notification
            var doctor = await _unitOfWork.Users.GetByIdAsync(request.DoctorId);
            var patient = await _unitOfWork.Users.GetByIdAsync(patientId);
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(request.ClinicId);

            // Notify doctor about new appointment
            await _notificationService.SendNotificationAsync(
                request.DoctorId,
                $"New appointment booked by {patient?.UserName} for {request.Date:MMM dd, yyyy} at {request.StartTime:HH:mm} at {clinic?.ClinicName}",
                "New Appointment Booked"
            );

            // Notify patient about appointment confirmation
            await _notificationService.SendNotificationAsync(
                patientId,
                $"Your appointment with Dr. {doctor?.UserName} is booked for {request.Date:MMM dd, yyyy} at {request.StartTime:HH:mm} at {clinic?.ClinicName}",
                "Appointment Confirmed"
            );

            return appointment;
        }

        public async Task<IEnumerable<AvailabilityResponseDto>> GetAllDoctorAvailabilityAsync()
        {
            var availability = await _unitOfWork.Availabilities.GetAllAsync("Doctor");
            return _mapper.Map<IEnumerable<AvailabilityResponseDto>>(availability);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync("Appointment.Patient,Appointment.Doctor");
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetTodayAppointmentsForDoctorAsync(int doctorId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var appointments = await _unitOfWork.Appointments.FindAsync(
                a => a.DoctorId == doctorId && a.AppointmentDate == today,
                "Patient,Doctor,Clinic");
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatusUpdateDto request)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, "Patient,Doctor,Clinic");
            if (appointment == null) throw new ArgumentException("Appointment not found");

            var oldStatus = appointment.Status;
            appointment.Status = request.Status;
            appointment.Reason = request.Reason;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Create notifications based on status change
            if (oldStatus != request.Status)
            {
                string statusText = request.Status.ToString();
                string message = "";
                string title = "";

                switch (request.Status)
                {
                    case AppointmentStatus.Completed:
                        message = $"Your appointment with Dr. {appointment.Doctor.UserName} on {appointment.AppointmentDate:MMM dd, yyyy} has been completed";
                        title = "Appointment Completed";
                        break;
                    case AppointmentStatus.Cancelled:
                        message = $"Your appointment with Dr. {appointment.Doctor.UserName} on {appointment.AppointmentDate:MMM dd, yyyy} has been cancelled";
                        title = "Appointment Cancelled";
                        break;
                    case AppointmentStatus.Booked:
                        message = $"Your appointment with Dr. {appointment.Doctor.UserName} on {appointment.AppointmentDate:MMM dd, yyyy} has been confirmed";
                        title = "Appointment Confirmed";
                        break;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    // Notify patient about status change
                    await _notificationService.SendNotificationAsync(
                        appointment.PatientId,
                        message,
                        title
                    );
                }
            }
        }

        public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, "Patient,Doctor,Clinic");
            if (appointment == null) throw new ArgumentException("Appointment not found");
            return _mapper.Map<AppointmentResponseDto>(appointment);
        }
    }
}