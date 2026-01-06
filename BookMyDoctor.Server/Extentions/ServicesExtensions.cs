using Microsoft.Extensions.DependencyInjection;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.Services.Implementations;
using BookMyDoctor.Server.Mappers;
using Kanini.LMP.Application.Services.Implementations;

namespace BookMyDoctor.Server.Extentions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // Memory Cache
            services.AddMemoryCache();

            // HttpClient for external APIs
            services.AddHttpClient();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Service registrations
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGeoService, GeoService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            
            return services;
        }
    }
}