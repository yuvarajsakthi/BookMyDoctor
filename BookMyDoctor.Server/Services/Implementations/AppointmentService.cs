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

        public async Task<IEnumerable<AppointmentResponseDto>> GetDoctorAppointmentsAsync(int doctorId)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(
                a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Pending,
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
                    case AppointmentStatus.Approved:
                        message = $"Your appointment with Dr. {appointment.Doctor.UserName} on {appointment.AppointmentDate:MMM dd, yyyy} has been approved";
                        title = "Appointment Approved";
                        break;
                    case AppointmentStatus.Rejected:
                        message = $"Your appointment with Dr. {appointment.Doctor.UserName} on {appointment.AppointmentDate:MMM dd, yyyy} has been rejected";
                        title = "Appointment Rejected";
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

        public async Task ApproveOrRejectAppointmentAsync(int appointmentId, AppointmentApprovalDto request, int doctorId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, "Patient,Doctor,Clinic");
            if (appointment == null) throw new ArgumentException("Appointment not found");
            if (appointment.DoctorId != doctorId) throw new UnauthorizedAccessException("Not authorized to modify this appointment");

            if (request.IsApproved)
            {
                appointment.Status = AppointmentStatus.Approved;
                await _notificationService.SendNotificationAsync(
                    appointment.PatientId,
                    $"Your appointment with Dr. {appointment.Doctor.UserName} on {appointment.AppointmentDate:MMM dd, yyyy} at {appointment.StartTime:HH:mm} has been approved",
                    "Appointment Approved"
                );
            }
            else
            {
                appointment.Status = AppointmentStatus.Rejected;
                appointment.Reason = request.Reason;
                
                await _notificationService.SendNotificationAsync(
                    appointment.PatientId,
                    $"Your appointment with Dr. {appointment.Doctor.UserName} on {appointment.AppointmentDate:MMM dd, yyyy} has been rejected. Reason: {request.Reason}",
                    "Appointment Rejected"
                );

                // Block the slot if requested
                if (request.BlockSlot)
                {
                    var blockedSlot = new Appointment
                    {
                        DoctorId = appointment.DoctorId,
                        ClinicId = appointment.ClinicId,
                        AppointmentDate = appointment.AppointmentDate,
                        StartTime = appointment.StartTime,
                        EndTime = appointment.EndTime,
                        Status = AppointmentStatus.Cancelled,
                        Reason = "BLOCKED_SLOT: " + (request.Reason ?? "Doctor unavailable"),
                        PatientId = appointment.DoctorId, // Use doctor as patient for blocked slots
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Appointments.AddAsync(blockedSlot);
                }
            }

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BlockTimeSlotAsync(BlockSlotDto request, int doctorId)
        {
            var blockedSlot = new Appointment
            {
                DoctorId = doctorId,
                ClinicId = request.ClinicId,
                AppointmentDate = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = AppointmentStatus.Cancelled,
                Reason = "BLOCKED_SLOT: " + (request.Reason ?? "Doctor unavailable"),
                PatientId = doctorId, // Use doctor as patient for blocked slots
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(blockedSlot);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CompleteAppointmentAsync(int appointmentId, int doctorId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, "Patient,Doctor,Clinic");
            if (appointment == null) throw new ArgumentException("Appointment not found");
            if (appointment.DoctorId != doctorId) throw new UnauthorizedAccessException("Not authorized to modify this appointment");
            if (appointment.AppointmentDate != DateOnly.FromDateTime(DateTime.Today)) 
                throw new InvalidOperationException("Can only complete appointments on the appointment date");

            appointment.Status = AppointmentStatus.Completed;
            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                appointment.PatientId,
                $"Your appointment with Dr. {appointment.Doctor.UserName} has been completed",
                "Appointment Completed"
            );
        }

        public async Task RescheduleAppointmentAsync(int appointmentId, AppointmentRescheduleDto request, int patientId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, "Patient,Doctor,Clinic");
            if (appointment == null) throw new ArgumentException("Appointment not found");
            if (appointment.PatientId != patientId) throw new UnauthorizedAccessException("Not authorized to modify this appointment");
            if (appointment.Status != AppointmentStatus.Approved && appointment.Status != AppointmentStatus.Booked)
                throw new InvalidOperationException("Can only reschedule approved or booked appointments");

            // Update appointment details and reset to pending for doctor approval
            appointment.AppointmentDate = request.NewDate;
            appointment.StartTime = request.NewStartTime;
            appointment.EndTime = request.NewStartTime.AddMinutes(30);
            appointment.Status = AppointmentStatus.Pending;
            appointment.Reason = request.Reason ?? appointment.Reason;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Notify doctor about reschedule request
            await _notificationService.SendNotificationAsync(
                appointment.DoctorId,
                $"Patient {appointment.Patient.UserName} has requested to reschedule appointment to {request.NewDate:MMM dd, yyyy} at {request.NewStartTime:HH:mm}",
                "Appointment Reschedule Request"
            );

            // Notify patient about reschedule submission
            await _notificationService.SendNotificationAsync(
                appointment.PatientId,
                $"Your reschedule request for appointment with Dr. {appointment.Doctor.UserName} has been submitted and is pending approval",
                "Reschedule Request Submitted"
            );
        }

        public async Task DoctorRescheduleAppointmentAsync(int appointmentId, DoctorRescheduleDto request, int doctorId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId, "Patient,Doctor,Clinic");
            if (appointment == null) throw new ArgumentException("Appointment not found");
            if (appointment.DoctorId != doctorId) throw new UnauthorizedAccessException("Not authorized to modify this appointment");
            if (appointment.Status != AppointmentStatus.Pending)
                throw new InvalidOperationException("Can only reschedule pending appointments");

            // Update appointment details and approve it
            appointment.AppointmentDate = request.NewDate;
            appointment.StartTime = request.NewStartTime;
            appointment.EndTime = request.NewStartTime.AddMinutes(30);
            appointment.Status = AppointmentStatus.Approved;
            appointment.Reason = request.Reason ?? appointment.Reason;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Notify patient about doctor's reschedule
            await _notificationService.SendNotificationAsync(
                appointment.PatientId,
                $"Dr. {appointment.Doctor.UserName} has rescheduled your appointment to {request.NewDate:MMM dd, yyyy} at {request.NewStartTime:HH:mm}. Reason: {request.Reason}",
                "Appointment Rescheduled by Doctor"
            );
        }
    }
}