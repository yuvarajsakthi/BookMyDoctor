using BookMyDoctor.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BookMyDoctor.Server.Data
{
    public class BmdContext : DbContext
    {
        public BmdContext(DbContextOptions<BmdContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorClinic> DoctorClinics { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PatientMedicalHistory> PatientMedicalHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all IEntityTypeConfiguration classes found in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Apply seed data
            Configurations.SeedData.Configure(modelBuilder);
        }
    }
}