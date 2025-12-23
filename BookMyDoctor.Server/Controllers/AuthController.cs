using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using System.Security.Claims;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;

        public AuthController(IAuthService authService, IOtpService otpService)
        {
            _authService = authService;
            _otpService = otpService;
        }

        [HttpPost("register/patient")]
        public async Task<ActionResult<ApiResponse<object>>> RegisterPatient([FromForm] PatientCreateDto request)
        {
            try
            {
                var user = await _authService.RegisterPatientAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(new { User = user }, "Patient registered successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("register/doctor")]
        public async Task<ActionResult<ApiResponse<object>>> RegisterDoctor([FromForm] DoctorCreateDto request)
        {
            try
            {
                var user = await _authService.RegisterDoctorAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(new { User = user }, "Doctor registered successfully. Awaiting approval."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("login/email")]
        public async Task<ActionResult<ApiResponse<object>>> LoginWithEmail([FromForm] LoginRequestDto request)
        {
            try
            {
                var (token, user) = await _authService.LoginWithEmailAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(new { Token = token, User = user }, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("login/email/otp")]
        public async Task<ActionResult<ApiResponse<object>>> LoginWithOtp([FromForm] VerifyOtpRequestDto request)
        {
            try
            {
                var (token, user) = await _authService.LoginWithOtpAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(new { Token = token, User = user }, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("otp/send")]
        public async Task<ActionResult<ApiResponse<object>>> SendOtp([FromForm] SendOtpRequestDto request)
        {
            try
            {
                await _authService.SendOtpAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "OTP sent successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("otp/verify")]
        public async Task<ActionResult<ApiResponse<object>>> VerifyOtp([FromForm] VerifyOtpRequestDto request)
        {
            try
            {
                var isValid = await _authService.VerifyOtpAsync(request);
                var message = isValid ? "OTP verified successfully" : "Invalid or expired OTP";
                
                return Ok(isValid ? 
                    ApiResponse<object>.SuccessResponse(null!, message) : 
                    ApiResponse<object>.ErrorResponse(message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Logged out successfully"));
        }

        [HttpPost("password/forgot")]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromForm] ForgotPasswordRequestDto request)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "If the email exists, a reset link has been sent"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("password/reset")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromForm] ResetPasswordWithOtpDto request)
        {
            try
            {
                await _authService.ResetPasswordWithOtpAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Password reset successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("password/change")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromForm] ChangePasswordRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _authService.ChangePasswordAsync(userId, request);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}