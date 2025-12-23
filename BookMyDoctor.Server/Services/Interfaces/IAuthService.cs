using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterPatientAsync(PatientCreateDto request);
        Task<UserResponseDto> RegisterDoctorAsync(DoctorCreateDto request);
        Task<(string token, UserResponseDto user)> LoginWithEmailAsync(LoginRequestDto request);
        Task<(string token, UserResponseDto user)> LoginWithOtpAsync(VerifyOtpRequestDto request);
        Task SendOtpAsync(SendOtpRequestDto request);
        Task<bool> VerifyOtpAsync(VerifyOtpRequestDto request);
        Task ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpDto request);
        Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    }
}