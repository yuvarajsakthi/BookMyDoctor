using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.Models
{
    public class Availability
    {
        public int AvailabilityId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; } = true;

        public User Doctor { get; set; } = null!;
    }
}