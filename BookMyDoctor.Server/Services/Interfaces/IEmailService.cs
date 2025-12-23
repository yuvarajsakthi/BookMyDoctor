using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailDto emailDto);
        Task<bool> SendOTPEmailAsync(OtpEmailDto dto);
    }
}