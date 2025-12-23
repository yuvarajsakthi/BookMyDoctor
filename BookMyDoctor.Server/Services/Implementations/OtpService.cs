using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.Templates;
using Microsoft.Extensions.Caching.Memory;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class OtpService : IOtpService
    {
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public OtpService(IEmailService emailService, IMemoryCache cache)
        {
            _emailService = emailService;
            _cache = cache;
        }

        public string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<bool> SendOTPAsync(OtpEmailDto otpEmailDto)
        {
            var emailDto = new EmailDto
            {
                To = otpEmailDto.Email,
                Subject = EmailTemplates.GetSubject(otpEmailDto.Purpose),
                Body = EmailTemplates.GetOtpEmailTemplate(otpEmailDto.UserName, otpEmailDto.OTP, otpEmailDto.Purpose),
                IsHtml = true
            };

            return await _emailService.SendEmailAsync(emailDto);
        }

        public bool ValidateOTP(string email, string otp, bool removeAfterValidation = false)
        {
            var key = $"otp_{email}";
            Console.WriteLine($"Validating OTP for {email}: {otp}");
            
            if (_cache.TryGetValue(key, out string? storedOtp))
            {
                Console.WriteLine($"Found stored OTP: {storedOtp}");
                if (storedOtp == otp)
                {
                    if (removeAfterValidation)
                    {
                        _cache.Remove(key);
                        Console.WriteLine($"OTP removed from cache for {email}");
                    }
                    return true;
                }
            }
            else
            {
                Console.WriteLine($"No OTP found for email: {email}");
            }
            return false;
        }

        public void StoreOTP(string email, string otp)
        {
            var key = $"otp_{email}";
            var expiry = DateTime.UtcNow.AddMinutes(5);
            _cache.Set(key, otp, TimeSpan.FromMinutes(5));
            Console.WriteLine($"Stored OTP for {email}: {otp}, Expiry: {expiry}");
        }
    }
}