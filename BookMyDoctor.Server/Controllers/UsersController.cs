using Microsoft.AspNetCore.Mvc;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _userService.GetDoctorsAsync();
            return Ok(new ApiResponse<IEnumerable<UserResponseDto>> { Success = true, Data = doctors });
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _userService.GetPatientsAsync();
            return Ok(new ApiResponse<IEnumerable<UserResponseDto>> { Success = true, Data = patients });
        }

        [HttpGet("doctors/{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _userService.GetDoctorByIdAsync(id);
            return Ok(new ApiResponse<UserResponseDto> { Success = true, Data = doctor });
        }

        [HttpGet("patients/{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _userService.GetPatientByIdAsync(id);
            return Ok(new ApiResponse<UserResponseDto> { Success = true, Data = patient });
        }

        [HttpGet("doctors/search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string? specialty, [FromQuery] string? location)
        {
            var doctors = await _userService.SearchDoctorsAsync(specialty, location);
            return Ok(new ApiResponse<IEnumerable<UserResponseDto>> { Success = true, Data = doctors });
        }

        [HttpGet("admin/dashboardsummary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var summary = await _userService.GetDashboardSummaryAsync();
            return Ok(new ApiResponse<DashboardSummaryDto> 
            { 
                Success = true, 
                Data = summary 
            });
        }

        [HttpGet("admin/appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _userService.GetAllAppointmentsAsync();
            return Ok(new ApiResponse<IEnumerable<AppointmentResponseDto>> { Success = true, Data = appointments });
        }

        [HttpGet("doctors/profile")]
        public async Task<IActionResult> GetDoctorProfile()
        {
            var doctorId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            var profile = await _userService.GetDoctorProfileAsync(doctorId);
            return Ok(new ApiResponse<UserResponseDto> { Success = true, Data = profile });
        }

        [HttpGet("patient/profile")]
        public async Task<IActionResult> GetPatientProfile()
        {
            var patientId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            var profile = await _userService.GetPatientProfileAsync(patientId);
            return Ok(new ApiResponse<UserResponseDto> { Success = true, Data = profile });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] object request)
        {
            await _userService.UpdateUserProfileAsync(id, request);
            return Ok(new ApiResponse<object> { Success = true });
        }
    }
}