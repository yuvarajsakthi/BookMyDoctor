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
            var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            return Ok(new ApiResponse<object> { Success = true, Data = appointments });
        }

        [HttpGet("doctor/pending")]
        public async Task<IActionResult> GetDoctorPendingAppointments()
        {
            var doctorId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
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

        [HttpPost("block-slot")]
        public async Task<IActionResult> BlockTimeSlot([FromBody] BlockSlotDto request)
        {
            var doctorId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            await _appointmentService.BlockTimeSlotAsync(request, doctorId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Time slot blocked successfully" });
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveOrRejectAppointment(int id, [FromBody] AppointmentApprovalDto request)
        {
            var doctorId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            await _appointmentService.ApproveOrRejectAppointmentAsync(id, request, doctorId);
            var action = request.IsApproved ? "approved" : "rejected";
            return Ok(new ApiResponse<object> { Success = true, Message = $"Appointment {action} successfully" });
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var doctorId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            await _appointmentService.CompleteAppointmentAsync(id, doctorId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Appointment completed successfully" });
        }

        [HttpPut("{id}/patient-reschedule")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] AppointmentRescheduleDto request)
        {
            var patientId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            await _appointmentService.RescheduleAppointmentAsync(id, request, patientId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Reschedule request submitted successfully" });
        }

        [HttpPut("{id}/doctor-reschedule")]
        public async Task<IActionResult> DoctorRescheduleAppointment(int id, [FromBody] DoctorRescheduleDto request)
        {
            var doctorId = int.Parse(User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0");
            await _appointmentService.DoctorRescheduleAppointmentAsync(id, request, doctorId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Appointment rescheduled successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            return Ok(new ApiResponse<object> { Success = true, Data = appointment });
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            return Ok(new ApiResponse<object> { Success = true });
        }
    }
}