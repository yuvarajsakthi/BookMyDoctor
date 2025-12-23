using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookMyDoctor.Server.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        [Required]
        public int UserId { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Bio { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ConsultationFee { get; set; }
        public bool IsApproved { get; set; } = false;
        [StringLength(100)]
        public string? Specialty { get; set; }
        
        // Availability fields
        public int? ClinicId { get; set; }
        public byte? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public User User { get; set; } = null!;
        public Clinic? Clinic { get; set; }
        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    }
}