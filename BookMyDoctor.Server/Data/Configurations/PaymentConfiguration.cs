using BookMyDoctor.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyDoctor.Server.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.PaymentId);
            builder.Property(p => p.RazorpayOrderId).HasMaxLength(100);
            builder.Property(p => p.RazorpayPaymentId).HasMaxLength(100);
            builder.Property(p => p.RazorpaySignature).HasMaxLength(100);
            builder.Property(p => p.Amount).HasColumnType("decimal(10,2)");
            builder.Property(p => p.Currency).HasMaxLength(3);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.ReceiptUrl).HasMaxLength(500);
            
            builder.HasOne(p => p.Appointment)
                   .WithMany()
                   .HasForeignKey(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}