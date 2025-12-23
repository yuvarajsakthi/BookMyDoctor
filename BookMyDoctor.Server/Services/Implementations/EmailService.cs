using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.Common;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = emailDto.Subject,
                    Body = emailDto.Body,
                    IsBodyHtml = emailDto.IsHtml
                };

                message.To.Add(new MailAddress(emailDto.To));

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {emailDto.To}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {emailDto.To}. Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendOTPEmailAsync(OtpEmailDto dto)
        {
            var subject = BookMyDoctor.Server.Templates.EmailTemplates.GetSubject(dto.Purpose);
            var body = BookMyDoctor.Server.Templates.EmailTemplates.GetOtpEmailTemplate(dto.UserName, dto.OTP, dto.Purpose);

            var emailDto = new EmailDto { To = dto.Email, Subject = subject, Body = body, IsHtml = true };
            return await SendEmailAsync(emailDto);
        }
    }
}