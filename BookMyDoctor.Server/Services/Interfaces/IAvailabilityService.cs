using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<IEnumerable<AvailabilityResponseDto>> GetDoctorAvailabilityAsync(int doctorId);
        Task<IEnumerable<object>> GetAvailableSlotsAsync(int doctorId, string date);
        Task AddAvailabilityAsync(int doctorId, AvailabilityDto request);
        Task RemoveAvailabilityAsync(int id);
        Task<IEnumerable<object>> GetDoctorBreaksAsync(int doctorId);
        Task AddBreakAsync(int doctorId, DoctorBreakDto request);
        Task RemoveBreakAsync(int id);
        Task<IEnumerable<object>> GetDoctorDaysOffAsync(int doctorId);
        Task AddDayOffAsync(int doctorId, DoctorDayOffDto request);
        Task RemoveDayOffAsync(int id);
        Task<object> GetAllAvailabilityDataAsync(int doctorId);
        Task<IEnumerable<object>> GetAllDoctorsAvailabilityAsync();
    }
}