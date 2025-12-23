using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("patients")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllPatients()
        {
            try
            {
                var patients = await _adminService.GetAllPatientsAsync();
                return Ok(ApiResponse<object>.SuccessResponse(patients, "Patients retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _adminService.GetAllDoctorsAsync();
                return Ok(ApiResponse<object>.SuccessResponse(doctors, "Doctors retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("patients/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdatePatient(int userId, [FromForm] PatientUpdateDto request)
        {
            try
            {
                var patient = await _adminService.UpdatePatientAsync(userId, request);
                return Ok(ApiResponse<object>.SuccessResponse(patient, "Patient updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("doctors/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateDoctor(int userId, [FromForm] DoctorUpdateDto request)
        {
            try
            {
                var doctor = await _adminService.UpdateDoctorAsync(userId, request);
                return Ok(ApiResponse<object>.SuccessResponse(doctor, "Doctor updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetUserDetails(int userId)
        {
            try
            {
                var user = await _adminService.GetUserDetailsAsync(userId);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                return Ok(ApiResponse<object>.SuccessResponse(user, "User details retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("users/{userId}/approval")]
        public async Task<ActionResult<ApiResponse<object>>> ApproveRejectDoctor(int userId, [FromForm] bool isApproved)
        {
            try
            {
                await _adminService.ApproveRejectDoctorAsync(userId, isApproved);
                var message = isApproved ? "Doctor approved" : "Doctor rejected";
                return Ok(ApiResponse<object>.SuccessResponse(null!, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("users/{userId}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUserStatus(int userId, [FromForm] bool isActive)
        {
            try
            {
                await _adminService.ActivateDeactivateUserAsync(userId, isActive);
                var message = isActive ? "User activated" : "User deactivated";
                return Ok(ApiResponse<object>.SuccessResponse(null!, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("clinics")]
        public async Task<ActionResult<ApiResponse<object>>> CreateClinic([FromForm] ClinicCreateDto request)
        {
            try
            {
                var clinic = await _adminService.CreateClinicAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(clinic, "Clinic created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("clinics")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllClinics()
        {
            try
            {
                var clinics = await _adminService.GetAllClinicsAsync();
                return Ok(ApiResponse<object>.SuccessResponse(clinics, "Clinics retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("clinics/{clinicId}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateClinic(int clinicId, [FromForm] ClinicUpdateDto request)
        {
            try
            {
                var clinic = await _adminService.UpdateClinicAsync(clinicId, request);
                return Ok(ApiResponse<object>.SuccessResponse(clinic, "Clinic updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("clinics/{clinicId}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteClinic(int clinicId)
        {
            try
            {
                await _adminService.DisableClinicAsync(clinicId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Clinic disabled successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("clinics/{clinicId}/doctors")]
        public async Task<ActionResult<ApiResponse<object>>> AssignDoctorToClinic(int clinicId, [FromForm] int doctorId)
        {
            try
            {
                await _adminService.AssignDoctorToClinicAsync(clinicId, doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Doctor assigned to clinic successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("clinics/{clinicId}/doctors/{doctorId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveDoctorFromClinic(int clinicId, int doctorId)
        {
            try
            {
                await _adminService.RemoveDoctorFromClinicAsync(clinicId, doctorId);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Doctor removed from clinic successfully"));
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
                var summary = await _adminService.GetDashboardSummaryAsync();
                return Ok(ApiResponse<object>.SuccessResponse(summary, "Dashboard summary retrieved"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("reports/appointments")]
        public async Task<ActionResult<ApiResponse<object>>> GetAppointmentReports([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            try
            {
                var appointments = await _adminService.GetAppointmentReportsAsync(from, to);
                return Ok(ApiResponse<object>.SuccessResponse(appointments, "Appointment report generated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("reports/revenue")]
        public async Task<ActionResult<ApiResponse<object>>> GetRevenueReport([FromQuery] int? clinicId)
        {
            try
            {
                var revenue = await _adminService.GetRevenueReportAsync(clinicId);
                return Ok(ApiResponse<object>.SuccessResponse(revenue, "Revenue report generated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("payments")]
        public async Task<ActionResult<ApiResponse<object>>> GetPayments([FromQuery] PaymentStatus? status)
        {
            try
            {
                var payments = await _adminService.GetPaymentsAsync(status);
                return Ok(ApiResponse<object>.SuccessResponse(payments, "Payments retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("payments/{paymentId}/reconcile")]
        public async Task<ActionResult<ApiResponse<object>>> ReconcilePayment(int paymentId, [FromForm] string? notes)
        {
            try
            {
                await _adminService.ReconcilePaymentAsync(paymentId, notes);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Payment reconciled successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}