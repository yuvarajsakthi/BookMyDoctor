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

        [HttpGet("clinic/{clinicId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorsByClinic(int clinicId)
        {
            var doctors = await _userService.GetDoctorsByClinicAsync(clinicId);
            return Ok(new ApiResponse<object> { Success = true, Data = doctors });
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchDoctors([FromQuery] string? specialty, [FromQuery] string? location, [FromQuery] DateTime? date)
        {
            try
            {
                var doctors = await _userService.SearchDoctorsAsync(specialty, location, date);
                return Ok(new ApiResponse<object> { Success = true, Data = doctors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("{doctorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorProfile(int doctorId)
        {
            try
            {
                var doctor = await _userService.GetDoctorByIdAsync(doctorId);
                if (doctor == null) return NotFound(new ApiResponse<object> { Success = false, Message = "Doctor not found" });
                return Ok(new ApiResponse<object> { Success = true, Data = doctor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] DoctorProfileUpdateDto request, IFormFile? profileImage)
        {
            try
            {
                var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var userId = int.Parse(userIdClaim ?? "0");
                
                await _userService.UpdateUserProfileAsync(userId, request, profileImage);
                return Ok(new ApiResponse<object> { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }
    }
}