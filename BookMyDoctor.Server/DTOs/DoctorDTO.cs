using BookMyDoctor.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.DTOs
{
    public class DoctorResponseDto
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public Gender? Gender { get; set; }
        public string? Specialty { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? Bio { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public byte? DayOfWeek { get; set; }
    }

    public class DayOffDto
    {
        [Required]
        public DateOnly Date { get; set; }
        public string? Reason { get; set; }
        public bool IsRecurring { get; set; } = false;
    }

    public class DoctorDashboardDto
    {
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public decimal TotalEarnings { get; set; }
        public double AverageRating { get; set; }
        public int TotalPatients { get; set; }
    }

    public class BlockTimeSlotDto
    {
        [Required]
        public int ClinicId { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        public string? Reason { get; set; }
    }

    public class SetAvailabilityDto
    {
        [Required]
        public int ClinicId { get; set; }
        [Required]
        public string DayOfWeek { get; set; } = null!;
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 15;
    }

    public class BlockedAppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Reason { get; set; }
    }
}