using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<IEnumerable<AvailabilityResponseDto>> GetDoctorAvailabilityAsync(int doctorId);
        Task<IEnumerable<object>> GetAvailableSlotsAsync(int doctorId, string date);
        Task AddAvailabilityAsync(int doctorId, AvailabilityDto request);
        Task RemoveAvailabilityAsync(int availabilityId);
    }
}