using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookMyDoctor.Server.Data.Configurations
{
    public static class SeedData
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    UserName = "Admin",
                    Email = "yuvarajsakthi003@gmail.com",
                    Phone = "1234567890",
                    PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj/VcSAyqfye", // Admin@123
                    UserRole = UserRole.Admin,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new User
                {
                    UserId = 2,
                    UserName = "Dr. Smith",
                    Email = "sakthiyuvaraj7@gmail.com",
                    Phone = "0987654321",
                    PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj/VcSAyqfye", // Admin@123
                    UserRole = UserRole.Doctor,
                    ExperienceYears = 10,
                    Bio = "Experienced cardiologist with 10 years of practice",
                    ConsultationFee = 500.00m,
                    IsApproved = true,
                    Specialty = "Cardiology",
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new User
                {
                    UserId = 3,
                    UserName = "John Doe",
                    Email = "yuvarajsakthi242003@gmail.com",
                    Phone = "5555555555",
                    PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj/VcSAyqfye", // Admin@123
                    UserRole = UserRole.Patient,
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    BloodGroup = BloodGroup.OPositive,
                    EmergencyContact = "9876543210",
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedAt = seedDate
                }
            );

            // Seed Clinics
            modelBuilder.Entity<Clinic>().HasData(
                new Clinic
                {
                    ClinicId = 1,
                    ClinicName = "City Medical Center",
                    Address = "123 Main Street",
                    City = "Chennai",
                    State = "Tamil Nadu",
                    Country = "India",
                    ZipCode = "600001",
                    IsActive = true
                },
                new Clinic
                {
                    ClinicId = 2,
                    ClinicName = "Health Plus Clinic",
                    Address = "456 Park Avenue",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    ZipCode = "400001",
                    IsActive = true
                }
            );

            // Seed Appointments
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);
            var nextWeek = today.AddDays(7);
            
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment
                {
                    AppointmentId = 1,
                    PatientId = 3,
                    DoctorId = 2,
                    ClinicId = 1,
                    AppointmentDate = new DateOnly(2024, 2, 15),
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(10, 30),
                    AppointmentType = AppointmentType.InPerson,
                    Status = AppointmentStatus.Completed,
                    Reason = "Regular checkup",
                    CreatedAt = seedDate
                },
                new Appointment
                {
                    AppointmentId = 2,
                    PatientId = 3,
                    DoctorId = 2,
                    ClinicId = 1,
                    AppointmentDate = tomorrow,
                    StartTime = new TimeOnly(14, 0),
                    EndTime = new TimeOnly(14, 30),
                    AppointmentType = AppointmentType.InPerson,
                    Status = AppointmentStatus.Booked,
                    Reason = "Follow-up consultation",
                    CreatedAt = DateTime.UtcNow
                },
                new Appointment
                {
                    AppointmentId = 3,
                    PatientId = 3,
                    DoctorId = 2,
                    ClinicId = 1,
                    AppointmentDate = nextWeek,
                    StartTime = new TimeOnly(11, 0),
                    EndTime = new TimeOnly(11, 30),
                    AppointmentType = AppointmentType.InPerson,
                    Status = AppointmentStatus.Pending,
                    Reason = "Routine examination",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed Payments
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    PaymentId = 1,
                    AppointmentId = 1,
                    UpiTransactionId = "upi_test123",
                    Amount = 500.00m,
                    Currency = "INR",
                    PaymentStatus = PaymentStatus.Paid,
                    Description = "Consultation fee",
                    CreatedAt = seedDate,
                    PaidAt = seedDate.AddMinutes(5)
                }
            );

            // Seed Notifications
            modelBuilder.Entity<Notification>().HasData(
                new Notification
                {
                    NotificationId = 1,
                    UserId = 3,
                    Message = "Your appointment has been confirmed",
                    SentAt = seedDate
                }
            );

            // Seed PatientMedicalHistories - removed
        }
    }
}