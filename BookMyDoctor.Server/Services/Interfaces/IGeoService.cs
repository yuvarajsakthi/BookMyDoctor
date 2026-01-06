using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IGeoService
    {
        Task<IEnumerable<NearbyClinicDto>> GetNearbyClinicsAsync(double userLatitude, double userLongitude, double radiusKm = 10);
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
        Task<bool> UpdateClinicCoordinatesAsync(int clinicId);
    }
}