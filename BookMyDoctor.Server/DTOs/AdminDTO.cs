using BookMyDoctor.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.DTOs
{
    public class FileUploadResponseDTO
    {
        public bool Success { get; set; }
        public string? PublicId { get; set; }
        public string? Url { get; set; }
        public string? SecureUrl { get; set; }
        public long? FileSize { get; set; }
        public string? Format { get; set; }
        public string? Message { get; set; }
    }

    public class PatientUpdateDto
    {
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public Gender? Gender { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }

    public class DoctorUpdateDto
    {
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public Gender? Gender { get; set; }
        public string? Specialty { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? Bio { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }

    public class ClinicWorkingHoursDto
    {
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public string? WorkingDays { get; set; }
    }

    public class ClinicHolidayDto
    {
        [Required]
        public DateOnly Date { get; set; }
        public string? Reason { get; set; }
    }

    public class ClinicCreateDto
    {
        [Required]
        public string ClinicName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
    }

    public class ClinicUpdateDto
    {
        public string? ClinicName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public bool? IsActive { get; set; }
    }

    public class InvoiceDto
    {
        [Required]
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    public class DashboardSummaryDto
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public Gender? Gender { get; set; }
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; }
        
        // Doctor specific fields
        public string? Specialty { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool? IsApproved { get; set; }
        
        // Patient specific fields
        public BloodGroup? BloodGroup { get; set; }
        public string? EmergencyContact { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }

    public class ClinicResponseDto
    {
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public bool IsActive { get; set; }
    }

    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string PatientName { get; set; } = null!;
        public string DoctorName { get; set; } = null!;
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? ClinicId { get; set; }
    }

    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
