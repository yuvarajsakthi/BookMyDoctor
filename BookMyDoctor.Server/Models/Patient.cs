using BookMyDoctor.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        [Required]
        public int UserId { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        
        [StringLength(10)]
        public BloodGroup? BloodGroup { get; set; }
        [StringLength(20)]
        public string? EmergencyContact { get; set; }
        
        public User User { get; set; } = null!;
        public ICollection<PatientMedicalHistory> MedicalHistories { get; set; } = new List<PatientMedicalHistory>();
    }
}