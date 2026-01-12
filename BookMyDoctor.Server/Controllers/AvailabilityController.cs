using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using System.Security.Claims;

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

        private int GetCurrentDoctorId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid user ID");
        }
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetDoctorAvailability(int doctorId)
        {
            try
            {
                var availability = await _availabilityService.GetDoctorAvailabilityAsync(doctorId);
                return Ok(new ApiResponse<object> { Success = true, Data = availability });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("doctor/{doctorId}/slots")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] string date)
        {
            try
            {
                var slots = await _availabilityService.GetAvailableSlotsAsync(doctorId, date);
                return Ok(new ApiResponse<object> { Success = true, Data = slots });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAvailabilityData()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                var allAvailability = await _availabilityService.GetAllAvailabilityDataAsync(doctorId);
                return Ok(new ApiResponse<object> { Success = true, Data = allAvailability });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAvailability([FromBody] AvailabilityDto availability)
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                await _availabilityService.AddAvailabilityAsync(doctorId, availability);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAvailability(int id)
        {
            try
            {
                await _availabilityService.RemoveAvailabilityAsync(id);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("breaks")]
        public async Task<IActionResult> GetDoctorBreaks()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                var breaks = await _availabilityService.GetDoctorBreaksAsync(doctorId);
                return Ok(new ApiResponse<object> { Success = true, Data = breaks });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("breaks")]
        public async Task<IActionResult> AddBreak([FromBody] DoctorBreakDto breakData)
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                await _availabilityService.AddBreakAsync(doctorId, breakData);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("breaks/{id}")]
        public async Task<IActionResult> RemoveBreak(int id)
        {
            try
            {
                await _availabilityService.RemoveBreakAsync(id);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("days-off")]
        public async Task<IActionResult> GetDoctorDaysOff()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                var daysOff = await _availabilityService.GetDoctorDaysOffAsync(doctorId);
                return Ok(new ApiResponse<object> { Success = true, Data = daysOff });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("days-off")]
        public async Task<IActionResult> AddDayOff([FromBody] DoctorDayOffDto dayOffData)
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                await _availabilityService.AddDayOffAsync(doctorId, dayOffData);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("days-off/{id}")]
        public async Task<IActionResult> RemoveDayOff(int id)
        {
            try
            {
                await _availabilityService.RemoveDayOffAsync(id);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }
    }
}