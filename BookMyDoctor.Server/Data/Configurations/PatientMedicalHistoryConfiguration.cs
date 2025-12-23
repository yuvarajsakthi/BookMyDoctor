using BookMyDoctor.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyDoctor.Server.Data.Configurations
{
    public class PatientMedicalHistoryConfiguration : IEntityTypeConfiguration<PatientMedicalHistory>
    {
        public void Configure(EntityTypeBuilder<PatientMedicalHistory> builder)
        {
            builder.HasKey(pmh => pmh.HistoryId);
            builder.Property(pmh => pmh.Condition).HasMaxLength(200);
            
            builder.HasOne(pmh => pmh.Patient)
                   .WithMany(p => p.MedicalHistories)
                   .HasForeignKey(pmh => pmh.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}