using System.ComponentModel.DataAnnotations;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public int ClinicId { get; set; }
        [Required]
        public DateOnly AppointmentDate { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        public AppointmentType? AppointmentType { get; set; }
        public AppointmentStatus? Status { get; set; }
        [StringLength(500)]
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public Clinic Clinic { get; set; } = null!;
    }
}