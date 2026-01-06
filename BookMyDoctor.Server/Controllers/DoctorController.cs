using Microsoft.AspNetCore.Mvc;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IUserService _userService;

        public DoctorController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var userId = int.Parse(userIdClaim ?? "0");
                var profile = await _userService.GetDoctorProfileAsync(userId);
                return Ok(new ApiResponse<object> { Success = true, Data = profile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] DoctorProfileUpdateDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var userId = int.Parse(userIdClaim ?? "0");
                
                var user = await _userService.GetUserProfileAsync(userId);
                if (user == null) return NotFound();
                
                // Update only allowed fields
                if (!string.IsNullOrEmpty(request.UserName)) user.UserName = request.UserName;
                if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
                if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
                if (request.Gender.HasValue) user.Gender = request.Gender;
                if (!string.IsNullOrEmpty(request.Specialty)) user.Specialty = request.Specialty;
                if (request.ExperienceYears.HasValue) user.ExperienceYears = request.ExperienceYears;
                if (request.ConsultationFee.HasValue) user.ConsultationFee = request.ConsultationFee;
                if (!string.IsNullOrEmpty(request.Bio)) user.Bio = request.Bio;
                
                await _userService.UpdateUserProfileAsync(userId, user);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }
    }
}