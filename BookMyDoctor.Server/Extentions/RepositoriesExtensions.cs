using Microsoft.EntityFrameworkCore;
using BookMyDoctor.Server.Data;
using BookMyDoctor.Server.Repositories.Interfaces;
using BookMyDoctor.Server.Repositories.Implementations;
using BookMyDoctor.Server.Repositories.UnitOfWork;

namespace BookMyDoctor.Server.Extentions
{
    public static class RepositoriesExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<BmdContext>(options =>
                options.UseSqlServer(connectionString));

            // Repository registrations
            services.AddScoped(typeof(IBmdRepository<>), typeof(BmdRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}