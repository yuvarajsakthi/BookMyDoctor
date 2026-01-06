using BookMyDoctor.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyDoctor.Server.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Phone).HasMaxLength(20);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(u => u.UserRole).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();
            
            // Patient-specific configurations
            builder.Property(u => u.EmergencyContact).HasMaxLength(20);
            
            // Doctor-specific configurations
            builder.Property(u => u.ConsultationFee).HasColumnType("decimal(10,2)");
            builder.Property(u => u.Specialty).HasMaxLength(100);
        }
    }
}