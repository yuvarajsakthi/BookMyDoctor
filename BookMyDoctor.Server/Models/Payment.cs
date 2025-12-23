using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int? AppointmentId { get; set; }
        
        // Razorpay specific fields
        [StringLength(100)]
        public string? RazorpayOrderId { get; set; }
        [StringLength(100)]
        public string? RazorpayPaymentId { get; set; }
        [StringLength(100)]
        public string? RazorpaySignature { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        [StringLength(3)]
        public string Currency { get; set; } = "INR";
        [StringLength(50)]
        public PaymentMethod? PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(500)]
        public string? ReceiptUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public Appointment? Appointment { get; set; }
    }
}