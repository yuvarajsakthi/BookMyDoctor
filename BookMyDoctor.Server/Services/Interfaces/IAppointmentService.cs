using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync();
        Task<Appointment> BookAppointmentAsync(int patientId, BookAppointmentDto request);
        Task<IEnumerable<AvailabilityResponseDto>> GetAllDoctorAvailabilityAsync();
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();
        Task<IEnumerable<AppointmentResponseDto>> GetTodayAppointmentsForDoctorAsync(int doctorId);
        Task UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatusUpdateDto request);
    }
}