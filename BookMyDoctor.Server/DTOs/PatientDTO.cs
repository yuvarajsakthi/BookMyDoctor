using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.DTOs
{
    public class PatientResponseDto
    {
        public int PatientId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public string? EmergencyContact { get; set; }
    }




    public class BookAppointmentDto
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public string? Reason { get; set; }
    }

    public class PatientRescheduleDto
    {
        public DateOnly NewDate { get; set; }
        public TimeOnly NewStartTime { get; set; }
        public string? Reason { get; set; }
    }

    public class PaymentCreateDto
    {
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class NearbyClinicDto
    {
        public int ClinicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DirectionsDto
    {
        public string Route { get; set; } = string.Empty;
        public int Duration { get; set; }
        public double Distance { get; set; }
    }

    public class StartConversationDto
    {
        public int DoctorId { get; set; }
        public string Subject { get; set; } = string.Empty;
    }

    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
    }

    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class EscalateDto
    {
        public int ConversationId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class AiQueryDto
    {
        public string Query { get; set; } = string.Empty;
        public int PatientId { get; set; }
    }

    public class AiResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public List<string> Suggestions { get; set; } = new();
    }

    public class FavoriteDto
    {
        public int FavoriteId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RecurringAppointmentDto
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class RecurringAppointmentResponseDto
    {
        public int RecurringId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public TimeOnly AppointmentTime { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class ReminderDto
    {
        public int AppointmentId { get; set; }
        public DateTime ReminderTime { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ReminderResponseDto
    {
        public int ReminderId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime ReminderTime { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}