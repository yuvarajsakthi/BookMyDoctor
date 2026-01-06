using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = null!;
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        [Required]
        [StringLength(500)]
        public string PasswordHash { get; set; } = null!;
        public string? ProfilePhotoPublicId { get; set; }
        public string? ProfileUrl { get; set; }
        public Gender? Gender { get; set; }
        [Required]
        public UserRole UserRole { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Patient-specific fields
        public DateOnly? DateOfBirth { get; set; }
        [StringLength(10)]
        public BloodGroup? BloodGroup { get; set; }
        [StringLength(20)]
        public string? EmergencyContact { get; set; }

        // Doctor-specific fields
        public int? ExperienceYears { get; set; }
        public string? Bio { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ConsultationFee { get; set; }
        public bool IsApproved { get; set; } = false;
        [StringLength(100)]
        public string? Specialty { get; set; }
        public int? ClinicId { get; set; }

        // Navigation properties
        public Clinic? Clinic { get; set; }
    }
}
