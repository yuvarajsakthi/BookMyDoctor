using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.Models.Enums;
using System.Security.Claims;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        private async Task<int> GetDoctorIdAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return await _doctorService.GetDoctorIdByUserIdAsync(userId);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<object>>> GetProfile()
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                if (doctorId == 0) return NotFound(ApiResponse<object>.ErrorResponse("Doctor not found"));

                var doctor = await _doctorService.GetProfileAsync(doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(doctor!, "Profile retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromForm] DoctorUpdateDto request)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                if (doctorId == 0) return NotFound(ApiResponse<object>.ErrorResponse("Doctor not found"));

                await _doctorService.UpdateProfileAsync(doctorId, request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Profile updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("clinics")]
        public async Task<ActionResult<ApiResponse<object>>> GetAssignedClinics()
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var clinics = await _doctorService.GetAssignedClinicsAsync(doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(clinics, "Clinics retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("availability")]
        public async Task<ActionResult<ApiResponse<object>>> SetAvailability([FromForm] SetAvailabilityDto request)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                await _doctorService.SetAvailabilityAsync(doctorId, request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Availability set"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("availability/block")]
        public async Task<ActionResult<ApiResponse<object>>> BlockTimeSlot([FromForm] BlockTimeSlotDto request)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var result = await _doctorService.BlockTimeSlotAsync(doctorId, request);
                return Ok(ApiResponse<object>.SuccessResponse(result, "Time slot blocked"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("availability/day-off")]
        public async Task<ActionResult<ApiResponse<object>>> MarkDayOff([FromForm] DayOffDto request)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                await _doctorService.MarkDayOffAsync(doctorId, request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Day marked as off"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("availability/calendar")]
        public async Task<ActionResult<ApiResponse<object>>> GetAvailabilityCalendar([FromQuery] int? clinicId, [FromQuery] string? month)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var appointments = await _doctorService.GetAvailabilityCalendarAsync(doctorId, clinicId, month);
                return Ok(ApiResponse<object>.SuccessResponse(appointments, "Calendar retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("appointments/today")]
        public async Task<ActionResult<ApiResponse<object>>> GetTodayAppointments()
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var appointments = await _doctorService.GetTodayAppointmentsAsync(doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(appointments, "Today's appointments"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("appointments")]
        public async Task<ActionResult<ApiResponse<object>>> GetAppointments([FromQuery] DateOnly? date, [FromQuery] AppointmentStatus? status)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var appointments = await _doctorService.GetAppointmentsAsync(doctorId, date, status);
                return Ok(ApiResponse<object>.SuccessResponse(appointments, "Appointments retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("appointments/{appointmentId}/accept")]
        public async Task<ActionResult<ApiResponse<object>>> AcceptAppointment(int appointmentId)
        {
            try
            {
                await _doctorService.AcceptAppointmentAsync(appointmentId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Appointment accepted"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("appointments/{appointmentId}/decline")]
        public async Task<ActionResult<ApiResponse<object>>> DeclineAppointment(int appointmentId, [FromForm] string? reason)
        {
            try
            {
                await _doctorService.DeclineAppointmentAsync(appointmentId, reason);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Appointment declined"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("availability/block/{blockId}")]
        public async Task<ActionResult<ApiResponse<object>>> FreeBlockedSlot(int blockId)
        {
            try
            {
                await _doctorService.FreeBlockedSlotAsync(blockId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Slot freed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("dashboard/summary")]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboardSummary()
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var summary = await _doctorService.GetDashboardSummaryAsync(doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(summary, "Dashboard summary"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("analytics/appointments")]
        public async Task<ActionResult<ApiResponse<object>>> GetAppointmentAnalytics([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var appointments = await _doctorService.GetAppointmentAnalyticsAsync(doctorId, from, to);
                return Ok(ApiResponse<object>.SuccessResponse(appointments, "Appointment analytics"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("analytics/cancellations")]
        public async Task<ActionResult<ApiResponse<object>>> GetCancellationAnalytics()
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var cancellations = await _doctorService.GetCancellationAnalyticsAsync(doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(cancellations, "Cancellation analytics"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("notifications")]
        public async Task<ActionResult<ApiResponse<object>>> GetNotifications()
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var notifications = await _doctorService.GetNotificationsAsync(doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(notifications, "Notifications retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("reports/export")]
        public async Task<ActionResult> ExportReports([FromQuery] string type, [FromQuery] string format)
        {
            try
            {
                var doctorId = await GetDoctorIdAsync();
                var data = await _doctorService.ExportReportsAsync(doctorId, type, format);
                return File(data, "application/octet-stream", $"report.{format}");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}