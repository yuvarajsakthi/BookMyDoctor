using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync();
        Task<IEnumerable<AppointmentResponseDto>> GetPatientAppointmentsAsync(int patientId);
        Task<IEnumerable<AppointmentResponseDto>> GetDoctorAppointmentsAsync(int doctorId);
        Task<Appointment> BookAppointmentAsync(int patientId, BookAppointmentDto request);
        Task<IEnumerable<AvailabilityResponseDto>> GetAllDoctorAvailabilityAsync();
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();
        Task<IEnumerable<AppointmentResponseDto>> GetTodayAppointmentsForDoctorAsync(int doctorId);
        Task UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatusUpdateDto request);
        Task<AppointmentResponseDto> GetAppointmentByIdAsync(int appointmentId);
        Task ApproveOrRejectAppointmentAsync(int appointmentId, AppointmentApprovalDto request, int doctorId);
        Task BlockTimeSlotAsync(BlockSlotDto request, int doctorId);
        Task CompleteAppointmentAsync(int appointmentId, int doctorId);
        Task RescheduleAppointmentAsync(int appointmentId, AppointmentRescheduleDto request, int patientId);
        Task DoctorRescheduleAppointmentAsync(int appointmentId, DoctorRescheduleDto request, int doctorId);
    }
}