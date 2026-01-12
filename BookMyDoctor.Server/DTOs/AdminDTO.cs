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

    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public string Date { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string DoctorName { get; set; } = null!;
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? ClinicId { get; set; }
    }

    public class DashboardSummaryDto
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<UserResponseDto> PendingDoctors { get; set; } = new List<UserResponseDto>();
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
        public string? ProfileUrl { get; set; }
        
        // Doctor specific fields
        public string? Specialty { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? Bio { get; set; }
        public bool? IsApproved { get; set; }
        
        // Patient specific fields
        public BloodGroup? BloodGroup { get; set; }
        public string? EmergencyContact { get; set; }
        public string? DateOfBirth { get; set; }
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

    public class NearbyClinicDto
    {
        public int ClinicId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double Distance { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AppointmentStatusUpdateDto
    {
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
    }

    public class AppointmentApprovalDto
    {
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
        public bool BlockSlot { get; set; } = false;
    }

    public class BlockSlotDto
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Reason { get; set; }
    }

    public class AppointmentRescheduleDto
    {
        public DateOnly NewDate { get; set; }
        public TimeOnly NewStartTime { get; set; }
        public string? Reason { get; set; }
    }

    public class DoctorRescheduleDto
    {
        public int AppointmentId { get; set; }
        public DateOnly NewDate { get; set; }
        public TimeOnly NewStartTime { get; set; }
        public string? Reason { get; set; }
    }

    public class BookAppointmentDto
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public string? Reason { get; set; }
    }

    public class AvailabilityDto
    {
        [Required]
        public int DayOfWeek { get; set; }
        [Required]
        public string StartTime { get; set; } = string.Empty;
        [Required]
        public string EndTime { get; set; } = string.Empty;
         public int? ClinicId { get; set; }
    }

    public class AvailabilityResponseDto
    {
        public int Id { get; set; }
        public int DayOfWeek { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string DoctorName { get; set; } = string.Empty;
    }
    
}
