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

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Service registrations
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuthService, AuthService>();
            
            return services;
        }
    }
}