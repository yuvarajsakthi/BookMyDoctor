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

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync("Patient,Doctor,Clinic");
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
            return appointment;
        }

        public async Task<IEnumerable<AvailabilityResponseDto>> GetAllDoctorAvailabilityAsync()
        {
            var availability = await _unitOfWork.Availabilities.GetAllAsync("Doctor");
            return _mapper.Map<IEnumerable<AvailabilityResponseDto>>(availability);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync("Appointment");
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
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null) throw new ArgumentException("Appointment not found");

            appointment.Status = request.Status;
            appointment.Reason = request.Reason;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}