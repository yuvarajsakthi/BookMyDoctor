using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.DTOs
{
    public class DoctorBreakDto
    {
        [Required]
        public int DayOfWeek { get; set; }
        
        [Required]
        public string StartTime { get; set; } = string.Empty;
        
        [Required]
        public string EndTime { get; set; } = string.Empty;
        
        public int? ClinicId { get; set; }
    }

    public class DoctorDayOffDto
    {
        [Required]
        public string Date { get; set; } = string.Empty;
        
        public string? Reason { get; set; }
    }
}