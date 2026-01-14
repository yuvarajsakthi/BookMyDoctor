using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetDoctorsAsync();
        Task<IEnumerable<UserResponseDto>> GetPatientsAsync();
        Task<UserResponseDto> GetDoctorByIdAsync(int id);
        Task<UserResponseDto> GetPatientByIdAsync(int id);
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<IEnumerable<UserResponseDto>> SearchDoctorsAsync(string? specialty, string? location, DateTime? date);
        Task<IEnumerable<UserResponseDto>> GetDoctorsByClinicAsync(int clinicId);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync();
        Task<UserResponseDto> GetDoctorProfileAsync(int doctorId);
        Task<UserResponseDto> GetPatientProfileAsync(int patientId);
        Task<User?> GetUserProfileAsync(int userId);
        Task UpdateUserProfileAsync(int userId, DoctorProfileUpdateDto request, IFormFile? profileImage);
    }
}