using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using System.Security.Claims;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ICloudinaryService _cloudinaryService;

        public PatientController(IPatientService patientService, ICloudinaryService cloudinaryService)
        {
            _patientService = patientService;
            _cloudinaryService = cloudinaryService;
        }

        private async Task<int> GetPatientIdAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return await _patientService.GetPatientIdByUserIdAsync(userId);
        }

        [HttpGet("doctors/search")]
        public async Task<ActionResult<ApiResponse<object>>> SearchDoctors([FromQuery] string? specialty, [FromQuery] string? location, [FromQuery] DateOnly? date, [FromQuery] TimeOnly? availableFrom, [FromQuery] TimeOnly? availableTo)
        {
            try
            {
                var doctors = await _patientService.SearchDoctorsAsync(specialty, location, date, availableFrom, availableTo);
                return Ok(ApiResponse<object>.SuccessResponse(doctors, "Doctors found"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<ApiResponse<object>>> GetDoctors()
        {
            try
            {
                var doctors = await _patientService.GetDoctorsAsync();
                return Ok(ApiResponse<object>.SuccessResponse(doctors, "Doctors retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("doctors/{doctorId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetDoctorProfile(int doctorId)
        {
            try
            {
                var doctor = await _patientService.GetDoctorProfileAsync(doctorId);
                if (doctor == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Doctor not found"));

                return Ok(ApiResponse<object>.SuccessResponse(doctor, "Doctor profile retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("doctors/{doctorId}/availability")]
        public async Task<ActionResult<ApiResponse<object>>> GetDoctorAvailability(int doctorId, [FromQuery] int? clinicId, [FromQuery] DateOnly? date)
        {
            try
            {
                var availability = await _patientService.GetDoctorAvailabilityAsync(doctorId, clinicId, date);
                return Ok(ApiResponse<object>.SuccessResponse(availability, "Availability retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("appointments")]
        public async Task<ActionResult<ApiResponse<object>>> BookAppointment([FromForm] BookAppointmentDto request)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                if (patientId == 0)
                    return NotFound(ApiResponse<object>.ErrorResponse("Patient not found"));

                var appointment = await _patientService.BookAppointmentAsync(patientId, request);
                return Ok(ApiResponse<object>.SuccessResponse(appointment, "Appointment booked successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("appointments/{appointmentId}/reschedule")]
        public async Task<ActionResult<ApiResponse<object>>> RescheduleAppointment(int appointmentId, [FromForm] PatientRescheduleDto request)
        {
            try
            {
                var appointment = await _patientService.RescheduleAppointmentAsync(appointmentId, request);
                return Ok(ApiResponse<object>.SuccessResponse(appointment, "Appointment rescheduled"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("appointments/{appointmentId}/cancel")]
        public async Task<ActionResult<ApiResponse<object>>> CancelAppointment(int appointmentId, [FromForm] string? reason)
        {
            try
            {
                await _patientService.CancelAppointmentAsync(appointmentId, reason);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Appointment cancelled"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("appointments/upcoming")]
        public async Task<ActionResult<ApiResponse<object>>> GetUpcomingAppointments()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var appointments = await _patientService.GetUpcomingAppointmentsAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(appointments, "Upcoming appointments retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("appointments/history")]
        public async Task<ActionResult<ApiResponse<object>>> GetAppointmentHistory()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var appointments = await _patientService.GetAppointmentHistoryAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(appointments, "Appointment history retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<object>>> GetProfile()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                if (patientId == 0)
                    return NotFound(ApiResponse<object>.ErrorResponse("Patient not found"));

                var patient = await _patientService.GetPatientProfileAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(patient!, "Profile retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromForm] PatientUpdateDto request)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                if (patientId == 0)
                    return NotFound(ApiResponse<object>.ErrorResponse("Patient not found"));

                await _patientService.UpdatePatientProfileAsync(patientId, request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Profile updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("profile/documents")]
        public async Task<ActionResult<ApiResponse<object>>> UploadDocument([FromForm] IFormFile file, [FromForm] string documentType)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var result = await _cloudinaryService.UploadDocumentAsync(file, userId!, documentType);
                if (result.Error != null)
                    return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));

                return Ok(ApiResponse<object>.SuccessResponse(new { Url = result.SecureUrl }, "Document uploaded"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("profile/documents")]
        public async Task<ActionResult<ApiResponse<object>>> GetMedicalRecords()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var records = await _patientService.GetMedicalRecordsAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(records, "Medical records retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("payments")]
        public async Task<ActionResult<ApiResponse<object>>> InitiatePayment([FromForm] PaymentCreateDto request)
        {
            try
            {
                var payment = await _patientService.InitiatePaymentAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(payment, "Payment initiated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("payments/{paymentId}/status")]
        public async Task<ActionResult<ApiResponse<object>>> GetPaymentStatus(int paymentId)
        {
            try
            {
                var status = await _patientService.GetPaymentStatusAsync(paymentId);
                return Ok(ApiResponse<object>.SuccessResponse(new { Status = status }, "Payment status retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("invoices")]
        public async Task<ActionResult<ApiResponse<object>>> GetInvoices()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var payments = await _patientService.GetInvoicesAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(payments, "Invoices retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("messages/conversations")]
        public async Task<ActionResult<ApiResponse<object>>> StartConversation([FromBody] StartConversationDto request)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var conversation = await _patientService.StartConversationAsync(patientId, request);
                return Ok(ApiResponse<object>.SuccessResponse(conversation, "Conversation started"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("messages/conversations")]
        public async Task<ActionResult<ApiResponse<object>>> GetConversations()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var conversations = await _patientService.GetConversationsAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(conversations, "Conversations retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("messages/conversations/{conversationId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetMessages(int conversationId)
        {
            try
            {
                var messages = await _patientService.GetMessagesAsync(conversationId);
                return Ok(ApiResponse<object>.SuccessResponse(messages, "Messages retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("messages")]
        public async Task<ActionResult<ApiResponse<object>>> SendMessage([FromForm] SendMessageDto request)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                await _patientService.SendMessageAsync(patientId, request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Message sent"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("assistant/query")]
        public async Task<ActionResult<ApiResponse<object>>> AskAI([FromForm] AiQueryDto request)
        {
            try
            {
                var response = await _patientService.ProcessAiQueryAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(response, "AI response generated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("favorites/doctors/{doctorId}")]
        public async Task<ActionResult<ApiResponse<object>>> AddFavoriteDoctor(int doctorId)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                await _patientService.AddFavoriteDoctorAsync(patientId, doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Doctor added to favorites"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("favorites/clinics/{clinicId}")]
        public async Task<ActionResult<ApiResponse<object>>> AddFavoriteClinic(int clinicId)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                await _patientService.AddFavoriteClinicAsync(patientId, clinicId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Clinic added to favorites"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("favorites")]
        public async Task<ActionResult<ApiResponse<object>>> GetFavorites()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var favorites = await _patientService.GetFavoritesAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(favorites, "Favorites retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("favorites/{favoriteId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveFavorite(int favoriteId)
        {
            try
            {
                await _patientService.RemoveFavoriteAsync(favoriteId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Favorite removed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("reminders")]
        public async Task<ActionResult<ApiResponse<object>>> SetReminder([FromForm] ReminderDto request)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var reminder = await _patientService.SetReminderAsync(patientId, request);
                return Ok(ApiResponse<object>.SuccessResponse(reminder, "Reminder set"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("reminders")]
        public async Task<ActionResult<ApiResponse<object>>> GetReminders()
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var reminders = await _patientService.GetRemindersAsync(patientId);
                return Ok(ApiResponse<object>.SuccessResponse(reminders, "Reminders retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("reminders/{reminderId}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteReminder(int reminderId)
        {
            try
            {
                await _patientService.DeleteReminderAsync(reminderId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Reminder deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}