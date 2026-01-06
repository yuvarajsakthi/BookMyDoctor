using Microsoft.AspNetCore.Mvc;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly IGeoService _geoService;

        public LocationController(IGeoService geoService)
        {
            _geoService = geoService;
        }

        [HttpGet("nearby-clinics")]
        public async Task<IActionResult> GetNearbyClinics([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 10)
        {
            var clinics = await _geoService.GetNearbyClinicsAsync(latitude, longitude, radiusKm);
            return Ok(new ApiResponse<object> { Success = true, Data = clinics });
        }
    }
}