using BookMyDoctor.Server.Data;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class GeoService : IGeoService
    {
        private readonly BmdContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeoService(BmdContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<NearbyClinicDto>> GetNearbyClinicsAsync(double userLatitude, double userLongitude, double radiusKm = 10)
        {
            var clinics = await _context.Clinics
                .Where(c => c.IsActive && c.Latitude.HasValue && c.Longitude.HasValue)
                .ToListAsync();

            var nearbyClinics = clinics
                .Select(c => new
                {
                    Clinic = c,
                    Distance = CalculateDistance(userLatitude, userLongitude, c.Latitude!.Value, c.Longitude!.Value)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Select(x => new NearbyClinicDto
                {
                    ClinicId = x.Clinic.ClinicId,
                    Name = x.Clinic.ClinicName,
                    Address = $"{x.Clinic.Address}, {x.Clinic.City}, {x.Clinic.State}",
                    Distance = Math.Round(x.Distance, 2),
                    Latitude = x.Clinic.Latitude!.Value,
                    Longitude = x.Clinic.Longitude!.Value
                });

            return nearbyClinics;
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371.0;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        public async Task<bool> UpdateClinicCoordinatesAsync(int clinicId)
        {
            var clinic = await _context.Clinics.FindAsync(clinicId);
            if (clinic == null || (clinic.Latitude.HasValue && clinic.Longitude.HasValue))
                return false;

            var fullAddress = $"{clinic.Address}, {clinic.City}, {clinic.State}, {clinic.Country}";
            var coordinates = await GetCoordinatesAsync(fullAddress);
            
            if (coordinates.HasValue)
            {
                clinic.Latitude = coordinates.Value.latitude;
                clinic.Longitude = coordinates.Value.longitude;
                await _context.SaveChangesAsync();
                return true;
            }
            
            return false;
        }

        private async Task<(double latitude, double longitude)?> GetCoordinatesAsync(string address)
        {
            try
            {
                var apiKey = _configuration["ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                    return null;

                var encodedAddress = Uri.EscapeDataString(address);
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";
                
                var response = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<GoogleGeocodeResponse>(response);
                
                if (result?.Status == "OK" && result.Results?.Any() == true)
                {
                    var location = result.Results[0].Geometry.Location;
                    return (location.Lat, location.Lng);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Geocoding failed for address '{address}': {ex.Message}");
            }
            
            return null;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }

    public class GoogleGeocodeResponse
    {
        public string Status { get; set; } = string.Empty;
        public GoogleGeocodeResult[] Results { get; set; } = Array.Empty<GoogleGeocodeResult>();
    }

    public class GoogleGeocodeResult
    {
        public GoogleGeometry Geometry { get; set; } = new();
    }

    public class GoogleGeometry
    {
        public GoogleLocation Location { get; set; } = new();
    }

    public class GoogleLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}