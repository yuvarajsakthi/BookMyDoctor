using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<IEnumerable<AvailabilityResponseDto>> GetDoctorAvailabilityAsync(int doctorId);
        Task AddAvailabilityAsync(int doctorId, AvailabilityDto request);
        Task RemoveAvailabilityAsync(int availabilityId);
    }
}