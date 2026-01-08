using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> CreatePayment([FromBody] CreatePaymentDto request)
        {
            try
            {
                var payment = await _paymentService.CreatePaymentAsync(request.AppointmentId, request.Amount);
                var response = _mapper.Map<PaymentResponseDto>(payment);
                
                return Ok(new ApiResponse<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment created successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<PaymentResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("verify")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> VerifyPayment([FromBody] VerifyPaymentDto request)
        {
            try
            {
                var payment = await _paymentService.VerifyPaymentAsync(
                    request.RazorpayOrderId, 
                    request.RazorpayPaymentId, 
                    request.RazorpaySignature);
                
                var response = _mapper.Map<PaymentResponseDto>(payment);
                
                return Ok(new ApiResponse<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment verified successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<PaymentResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("cancel/{appointmentId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> CancelPayment(int appointmentId)
        {
            try
            {
                var payment = await _paymentService.CancelPaymentAsync(appointmentId);
                var response = _mapper.Map<PaymentResponseDto>(payment);
                
                return Ok(new ApiResponse<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment cancelled successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<PaymentResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetPaymentByAppointment(int appointmentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByAppointmentIdAsync(appointmentId);
                if (payment == null)
                {
                    return NotFound(new ApiResponse<PaymentResponseDto>
                    {
                        Success = false,
                        Message = "Payment not found"
                    });
                }

                var response = _mapper.Map<PaymentResponseDto>(payment);
                
                return Ok(new ApiResponse<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment retrieved successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<PaymentResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("complete/{appointmentId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> CompletePaymentAfterAppointment(int appointmentId)
        {
            try
            {
                // This endpoint allows patients to pay after appointment completion
                var payment = await _paymentService.GetPaymentByAppointmentIdAsync(appointmentId);
                if (payment == null)
                {
                    // Create payment if it doesn't exist (for appointments completed without prior payment)
                    payment = await _paymentService.CreatePaymentAsync(appointmentId, 500m);
                }

                var response = _mapper.Map<PaymentResponseDto>(payment);
                
                return Ok(new ApiResponse<PaymentResponseDto>
                {
                    Success = true,
                    Message = "Payment ready for completion",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<PaymentResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var payments = new object[] { };
            return Ok(new ApiResponse<object> { Success = true, Data = payments });
        }

        [HttpGet("invoice/{paymentId}")]
        public async Task<IActionResult> DownloadInvoice(int paymentId)
        {
            var pdfBytes = new byte[] { }; // Mock PDF data
            return File(pdfBytes, "application/pdf", $"invoice-{paymentId}.pdf");
        }
    }
}