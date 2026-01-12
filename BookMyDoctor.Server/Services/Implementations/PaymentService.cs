using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;
using PaymentModel = BookMyDoctor.Server.Models.Payment;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RazorpayClient _razorpayClient;
        private readonly string _razorpayKeySecret;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            var keyId = Environment.GetEnvironmentVariable("RAZORPAY_KEY_ID");
            _razorpayKeySecret = Environment.GetEnvironmentVariable("RAZORPAY_KEY_SECRET") ?? "";
            _razorpayClient = new RazorpayClient(keyId, _razorpayKeySecret);
        }

        public async Task<PaymentModel> CreatePaymentAsync(int appointmentId, decimal amount)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new ArgumentException("Appointment not found");

            // Create Razorpay order
            var orderOptions = new Dictionary<string, object>
            {
                {"amount", (int)(amount * 100)}, // Amount in paise
                {"currency", "INR"},
                {"receipt", $"appointment_{appointmentId}_{DateTime.UtcNow.Ticks}"}
            };

            var order = _razorpayClient.Order.Create(orderOptions);

            // Create payment record
            var payment = new PaymentModel
            {
                AppointmentId = appointmentId,
                RazorpayOrderId = order["id"].ToString(),
                Amount = amount,
                Currency = "INR",
                PaymentStatus = PaymentStatus.Pending,
                Description = $"Payment for appointment with Dr. {appointment.Doctor?.UserName}",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return payment;
        }

        public async Task<PaymentModel> VerifyPaymentAsync(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature)
        {
            // Verify signature
            var isValidSignature = VerifyRazorpaySignature(razorpayOrderId, razorpayPaymentId, razorpaySignature);
            if (!isValidSignature)
                throw new ArgumentException("Invalid payment signature");

            // Find payment by order ID
            var payments = await _unitOfWork.Payments.FindAsync(p => p.RazorpayOrderId == razorpayOrderId);
            var payment = payments.FirstOrDefault();
            if (payment == null)
                throw new ArgumentException("Payment not found");

            // Update payment status
            payment.RazorpayPaymentId = razorpayPaymentId;
            payment.PaymentStatus = PaymentStatus.Paid;
            payment.PaidAt = DateTime.UtcNow;

            await _unitOfWork.Payments.UpdateAsync(payment);

            // Update appointment status to PaymentDone
            if (payment.AppointmentId.HasValue)
            {
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(payment.AppointmentId.Value);
                if (appointment != null)
                {
                    appointment.Status = AppointmentStatus.PaymentDone;
                    await _unitOfWork.Appointments.UpdateAsync(appointment);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return payment;
        }

        public async Task<PaymentModel> CancelPaymentAsync(int appointmentId)
        {
            var payments = await _unitOfWork.Payments.FindAsync(p => p.AppointmentId == appointmentId);
            var payment = payments.FirstOrDefault();
            if (payment == null)
                throw new ArgumentException("Payment not found");

            if (payment.PaymentStatus == PaymentStatus.Paid)
            {
                // Process refund if payment was completed
                payment.PaymentStatus = PaymentStatus.Refunded;
            }
            else
            {
                // Cancel pending payment
                payment.PaymentStatus = PaymentStatus.Cancelled;
            }

            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return payment;
        }

        public async Task<PaymentModel?> GetPaymentByAppointmentIdAsync(int appointmentId)
        {
            var payments = await _unitOfWork.Payments.FindAsync(p => p.AppointmentId == appointmentId);
            return payments.FirstOrDefault();
        }

        public async Task<PaymentModel?> GetPaymentByIdAsync(int paymentId)
        {
            return await _unitOfWork.Payments.GetByIdAsync(paymentId);
        }

        public async Task<IEnumerable<PaymentModel>> GetPaymentHistoryByUserAsync(int userId)
        {
            // Get all appointments for the user (patient)
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == userId);
            var appointmentIds = appointments.Select(a => a.AppointmentId).ToList();
            
            // Get all payments for these appointments
            var payments = await _unitOfWork.Payments.FindAsync(p => appointmentIds.Contains(p.AppointmentId ?? 0));
            
            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<IEnumerable<PaymentModel>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            return payments.OrderByDescending(p => p.CreatedAt);
        }

        private bool VerifyRazorpaySignature(string orderId, string paymentId, string signature)
        {
            var payload = $"{orderId}|{paymentId}";
            var secret = _razorpayKeySecret;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

            return computedSignature == signature.ToLower();
        }
    }
}