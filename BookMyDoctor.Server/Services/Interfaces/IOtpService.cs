using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IOtpService
    {
        string GenerateOTP();
        Task<bool> SendOTPAsync(OtpEmailDto otpEmailDto);
        bool ValidateOTP(string email, string otp, bool removeAfterValidation = false);
        void StoreOTP(string email, string otp);
    }
}