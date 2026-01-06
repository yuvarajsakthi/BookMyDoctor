using BookMyDoctor.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.DTOs
{
    public class DoctorProfileUpdateDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Gender? Gender { get; set; }
        public string? Specialty { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? Bio { get; set; }
    }
}