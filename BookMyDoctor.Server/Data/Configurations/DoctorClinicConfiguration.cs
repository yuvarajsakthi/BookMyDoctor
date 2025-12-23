using BookMyDoctor.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyDoctor.Server.Data.Configurations
{
    public class DoctorClinicConfiguration : IEntityTypeConfiguration<DoctorClinic>
    {
        public void Configure(EntityTypeBuilder<DoctorClinic> builder)
        {
            // Composite primary key
            builder.HasKey(dc => new { dc.DoctorId, dc.ClinicId });
            
            // Configure many-to-many relationship
            builder.HasOne(dc => dc.Doctor)
                   .WithMany(d => d.DoctorClinics)
                   .HasForeignKey(dc => dc.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);
                   
            builder.HasOne(dc => dc.Clinic)
                   .WithMany(c => c.DoctorClinics)
                   .HasForeignKey(dc => dc.ClinicId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}