using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.DTOs
{
    public class EmailDto
    {
        public string To { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsHtml { get; set; } = false;
    }

        public class OtpEmailDto
    {
        public string Email { get; set; } = null!;
        public string OTP { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public OtpPurpose Purpose { get; set; }
    }
}