using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Templates
{
    public static class EmailTemplates
    {
        public static string GetOtpEmailTemplate(string userName, string otp, OtpPurpose purpose)
        {
            var action = purpose switch
            {
                OtpPurpose.LOGIN => "login to your account",
                OtpPurpose.REGISTER => "complete your registration",
                OtpPurpose.FORGET_PASSWORD => "reset your password",
                _ => "verify your account"
            };

            return $@"
                <html>
                <body>
                    <h2>BookMyDoctor OTP Verification</h2>
                    <p>Hello {userName},</p>
                    <p>Your OTP to {action} is:</p>
                    <h3 style='color: #007bff; font-size: 24px;'>{otp}</h3>
                    <p>This OTP is valid for 5 minutes.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <br>
                    <p>Best regards,<br>BookMyDoctor Team</p>
                </body>
                </html>";
        }

        public static string GetSubject(OtpPurpose purpose)
        {
            return purpose switch
            {
                OtpPurpose.LOGIN => "BookMyDoctor - Login OTP",
                OtpPurpose.REGISTER => "BookMyDoctor - Registration OTP",
                OtpPurpose.FORGET_PASSWORD => "BookMyDoctor - Password Reset OTP",
                _ => "BookMyDoctor - OTP Verification"
            };
        }
    }
}