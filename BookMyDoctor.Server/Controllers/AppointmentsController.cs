using Microsoft.AspNetCore.Mvc;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(new ApiResponse<object> { Success = true, Data = appointments });
        }

        [HttpGet("availability")]
        public async Task<IActionResult> GetDoctorAvailability()
        {
            var availability = await _appointmentService.GetAllDoctorAvailabilityAsync();
            return Ok(new ApiResponse<object> { Success = true, Data = availability });
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _appointmentService.GetAllPaymentsAsync();
            return Ok(new ApiResponse<object> { Success = true, Data = payments });
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto request)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var appointment = await _appointmentService.BookAppointmentAsync(userId, request);
            return Ok(new ApiResponse<object> { Success = true, Data = appointment });
        }

        [HttpGet("patient")]
        public async Task<IActionResult> GetPatientAppointments()
        {
            var patientId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(new ApiResponse<object> { Success = true, Data = appointments });
        }

        [HttpGet("today/doctor")]
        public async Task<IActionResult> GetTodayAppointmentsForDoctor()
        {
            var doctorId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            var appointments = await _appointmentService.GetTodayAppointmentsForDoctorAsync(doctorId);
            return Ok(new ApiResponse<object> { Success = true, Data = appointments });
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] AppointmentStatusUpdateDto request)
        {
            await _appointmentService.UpdateAppointmentStatusAsync(id, request);
            return Ok(new ApiResponse<object> { Success = true });
        }
    }
}