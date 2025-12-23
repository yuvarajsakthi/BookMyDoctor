using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorResponseDto?> GetProfileAsync(int doctorId);
        Task UpdateProfileAsync(int doctorId, DoctorUpdateDto request);
        Task<IEnumerable<ClinicResponseDto>> GetAssignedClinicsAsync(int doctorId);
        Task SetAvailabilityAsync(int doctorId, SetAvailabilityDto request);
        Task<BlockedAppointmentResponseDto> BlockTimeSlotAsync(int doctorId, BlockTimeSlotDto request);
        Task MarkDayOffAsync(int doctorId, DayOffDto request);
        Task<IEnumerable<AppointmentResponseDto>> GetAvailabilityCalendarAsync(int doctorId, int? clinicId, string? month);
        Task<IEnumerable<AppointmentResponseDto>> GetTodayAppointmentsAsync(int doctorId);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsAsync(int doctorId, DateOnly? date, AppointmentStatus? status);
        Task AcceptAppointmentAsync(int appointmentId);
        Task DeclineAppointmentAsync(int appointmentId, string? reason);
        Task FreeBlockedSlotAsync(int blockId);
        Task<DoctorDashboardDto> GetDashboardSummaryAsync(int doctorId);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentAnalyticsAsync(int doctorId, DateOnly from, DateOnly to);
        Task<IEnumerable<AppointmentResponseDto>> GetCancellationAnalyticsAsync(int doctorId);
        Task<int> GetDoctorIdByUserIdAsync(int userId);
        Task<IEnumerable<object>> GetNotificationsAsync(int doctorId);
        Task<byte[]> ExportReportsAsync(int doctorId, string type, string format);
    }
}