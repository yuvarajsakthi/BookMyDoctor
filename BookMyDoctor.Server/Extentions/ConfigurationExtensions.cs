using BookMyDoctor.Server.Common;
using CloudinaryDotNet;

namespace BookMyDoctor.Server.Extentions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services)
        {
            // Configure strongly-typed settings from environment variables
            services.Configure<SmtpSettings>(options =>
            {
                options.Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "";
                options.Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
                options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
                options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
                options.FromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? "";
                options.FromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "";
            });

            services.Configure<RazorpaySettings>(options =>
            {
                options.KeyId = Environment.GetEnvironmentVariable("RAZORPAY_KEY_ID") ?? "";
                options.KeySecret = Environment.GetEnvironmentVariable("RAZORPAY_KEY_SECRET") ?? "";
            });

            services.Configure<JwtSettings>(options =>
            {
                options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
                options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "";
                options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "";
                options.ExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? "60");
            });

            // OpenStreetMap - No API key needed
            Console.WriteLine("Using OpenStreetMap for location services (no API key required)");

            // Configure Cloudinary
            var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;
            services.AddSingleton(cloudinary);

            return services;
        }
    }
}