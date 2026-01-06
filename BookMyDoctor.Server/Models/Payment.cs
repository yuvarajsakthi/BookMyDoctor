using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int? AppointmentId { get; set; }
        
        [StringLength(100)]
        public string? RazorpayOrderId { get; set; }
        
        [StringLength(100)]
        public string? RazorpayPaymentId { get; set; }
        
        [StringLength(100)]
        public string? UpiTransactionId { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        [StringLength(3)]
        public string Currency { get; set; } = "INR";
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        [StringLength(500)]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public Appointment? Appointment { get; set; }
    }
}