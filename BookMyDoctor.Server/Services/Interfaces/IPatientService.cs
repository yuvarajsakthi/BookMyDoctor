using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<DoctorResponseDto>> SearchDoctorsAsync(string? specialty, string? location, DateOnly? date, TimeOnly? availableFrom, TimeOnly? availableTo);
        Task<IEnumerable<DoctorResponseDto>> GetDoctorsAsync();
        Task<DoctorResponseDto?> GetDoctorProfileAsync(int doctorId);
        Task<object> GetDoctorAvailabilityAsync(int doctorId, int? clinicId, DateOnly? date);
        Task<Appointment> BookAppointmentAsync(int patientId, BookAppointmentDto request);
        Task<Appointment> RescheduleAppointmentAsync(int appointmentId, PatientRescheduleDto request);
        Task CancelAppointmentAsync(int appointmentId, string? reason);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentHistoryAsync(int patientId);
        Task<PatientResponseDto?> GetPatientProfileAsync(int patientId);
        Task UpdatePatientProfileAsync(int patientId, PatientUpdateDto request);
        Task<IEnumerable<PatientMedicalHistory>> GetMedicalRecordsAsync(int patientId);
        Task<Payment> InitiatePaymentAsync(PaymentCreateDto request);
        Task<PaymentStatus> GetPaymentStatusAsync(int paymentId);
        Task<IEnumerable<Payment>> GetInvoicesAsync(int patientId);
        Task<int> GetPatientIdByUserIdAsync(int userId);

        // Messaging
        Task<object> StartConversationAsync(int patientId, StartConversationDto request);
        Task<IEnumerable<ConversationDto>> GetConversationsAsync(int patientId);
        Task<IEnumerable<MessageResponseDto>> GetMessagesAsync(int conversationId);
        Task SendMessageAsync(int patientId, SendMessageDto request);

        // AI Assistant
        Task<AiResponseDto> ProcessAiQueryAsync(AiQueryDto request);

        // Favorites
        Task AddFavoriteDoctorAsync(int patientId, int doctorId);
        Task AddFavoriteClinicAsync(int patientId, int clinicId);
        Task<IEnumerable<FavoriteDto>> GetFavoritesAsync(int patientId);
        Task RemoveFavoriteAsync(int favoriteId);

        // Reminders
        Task<ReminderResponseDto> SetReminderAsync(int patientId, ReminderDto request);
        Task<IEnumerable<ReminderResponseDto>> GetRemindersAsync(int patientId);
        Task DeleteReminderAsync(int reminderId);
    }
}