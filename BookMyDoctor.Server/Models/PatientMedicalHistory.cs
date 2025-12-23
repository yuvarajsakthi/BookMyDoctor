using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.Models
{
    public class PatientMedicalHistory
    {
        public int HistoryId { get; set; }
        public int? PatientId { get; set; }
        [StringLength(200)]
        public string? Condition { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Patient? Patient { get; set; }
    }
}