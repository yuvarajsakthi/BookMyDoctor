using Microsoft.AspNetCore.Mvc;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorAvailability()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var doctorId = int.Parse(userIdClaim ?? "0");
            var availability = await _availabilityService.GetDoctorAvailabilityAsync(doctorId);
            return Ok(new ApiResponse<object> { Success = true, Data = availability });
        }

        [HttpPost]
        public async Task<IActionResult> AddAvailability([FromBody] AvailabilityDto request)
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var doctorId = int.Parse(userIdClaim ?? "0");
            await _availabilityService.AddAvailabilityAsync(doctorId, request);
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAvailability(int id)
        {
            await _availabilityService.RemoveAvailabilityAsync(id);
            return Ok(new ApiResponse<object> { Success = true });
        }
    }
}