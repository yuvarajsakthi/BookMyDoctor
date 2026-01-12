using BookMyDoctor.Server.Models;
using PaymentModel = BookMyDoctor.Server.Models.Payment;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentModel> CreatePaymentAsync(int appointmentId, decimal amount);
        Task<PaymentModel> VerifyPaymentAsync(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature);
        Task<PaymentModel> CancelPaymentAsync(int appointmentId);
        Task<PaymentModel?> GetPaymentByAppointmentIdAsync(int appointmentId);
        Task<PaymentModel?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<PaymentModel>> GetPaymentHistoryByUserAsync(int userId);
        Task<IEnumerable<PaymentModel>> GetAllPaymentsAsync();
    }
}