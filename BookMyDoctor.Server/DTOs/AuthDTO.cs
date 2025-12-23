using BookMyDoctor.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.DTOs
{
    public class PatientCreateDto
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        [Required]
        public string Password { get; set; } = null!;
        public Gender? Gender { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }

    public class DoctorCreateDto
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        [Required]
        public string Password { get; set; } = null!;
        public Gender? Gender { get; set; }
        [Required]
        public string Specialty { get; set; } = null!;
        [Required]
        public int ExperienceYears { get; set; }

    }
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class LoginOtpRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class SendOtpRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public OtpPurpose Purpose { get; set; }
    }

    public class VerifyOtpRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string Otp { get; set; } = null!;
    }

    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
    
    public class ResetPasswordWithOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string Otp { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;
    }

    public class ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public Gender? Gender { get; set; }
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; }
    }

    public class PatientDto
    {
        public int UserId { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public string? EmergencyContact { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }

    public class DoctorDto
    {
        public int UserId { get; set; }
        public string? Specialty { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}