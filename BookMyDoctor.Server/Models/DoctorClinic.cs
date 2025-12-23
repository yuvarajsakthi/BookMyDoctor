using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.Models
{
    public class DoctorClinic
    {
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public int ClinicId { get; set; }
        
        public Doctor Doctor { get; set; } = null!;
        public Clinic Clinic { get; set; } = null!;
    }
}