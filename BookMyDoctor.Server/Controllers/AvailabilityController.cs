using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvailabilityController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDoctorAvailability()
        {
            // Mock data for now
            var availability = new[]
            {
                new { id = 1, clinicId = 1, dayOfWeek = 1, startTime = "09:00", endTime = "17:00" },
                new { id = 2, clinicId = 1, dayOfWeek = 2, startTime = "09:00", endTime = "17:00" }
            };
            return Ok(new ApiResponse<object> { Success = true, Data = availability });
        }

        [HttpPost]
        public async Task<IActionResult> AddAvailability([FromBody] object availability)
        {
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAvailability(int id)
        {
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpGet("breaks")]
        public async Task<IActionResult> GetDoctorBreaks()
        {
            var breaks = new object[] { };
            return Ok(new ApiResponse<object> { Success = true, Data = breaks });
        }

        [HttpPost("breaks")]
        public async Task<IActionResult> AddBreak([FromBody] object breakData)
        {
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpDelete("breaks/{id}")]
        public async Task<IActionResult> RemoveBreak(int id)
        {
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpGet("days-off")]
        public async Task<IActionResult> GetDoctorDaysOff()
        {
            var daysOff = new object[] { };
            return Ok(new ApiResponse<object> { Success = true, Data = daysOff });
        }

        [HttpPost("days-off")]
        public async Task<IActionResult> AddDayOff([FromBody] object dayOffData)
        {
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpDelete("days-off/{id}")]
        public async Task<IActionResult> RemoveDayOff(int id)
        {
            return Ok(new ApiResponse<object> { Success = true });
        }
    }
}