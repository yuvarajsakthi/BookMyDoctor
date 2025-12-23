using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.Models
{
    public class Clinic
    {
        public int ClinicId { get; set; }
        [Required]
        [StringLength(200)]
        public string ClinicName { get; set; } = null!;
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(100)]
        public string? State { get; set; }
        [StringLength(100)]
        public string? Country { get; set; }
        [StringLength(20)]
        public string? ZipCode { get; set; }
        public bool IsActive { get; set; } = true;
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public string? WorkingDays { get; set; }
        public string? HolidayDates { get; set; }

        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    }
}