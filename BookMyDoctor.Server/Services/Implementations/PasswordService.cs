using System.Text.RegularExpressions;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class PasswordService
    {
        public static string HashPassword(string password)
        {
            ValidatePassword(password);
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                throw new ArgumentException("Password must contain at least one uppercase letter");

            if (!Regex.IsMatch(password, @"[a-z]"))
                throw new ArgumentException("Password must contain at least one lowercase letter");

            if (!Regex.IsMatch(password, @"\d"))
                throw new ArgumentException("Password must contain at least one digit");

            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
                throw new ArgumentException("Password must contain at least one special character");
        }
    }
}