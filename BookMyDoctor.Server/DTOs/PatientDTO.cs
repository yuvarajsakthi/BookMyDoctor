using BookMyDoctor.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.DTOs
{
    public class PatientRescheduleDto
    {
        [Required]
        public DateOnly NewDate { get; set; }
        [Required]
        public TimeOnly NewStartTime { get; set; }
        public string? Reason { get; set; }
    }
}