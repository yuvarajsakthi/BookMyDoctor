using System.ComponentModel.DataAnnotations;
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
    }
}
