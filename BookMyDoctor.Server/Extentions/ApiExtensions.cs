using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Text;

namespace BookMyDoctor.Server.Extentions
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Serilog
            services.AddSerilog((serviceProvider, loggerConfiguration) =>
            {
                loggerConfiguration
                    .MinimumLevel.Warning()
                    .WriteTo.Console(outputTemplate: "{Level:u3}: {Message:lj}{NewLine}")
                    .WriteTo.File(
                        path: "Logs/bmd-log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7
                    );
            });

            // Controllers with JSON configuration
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            // JWT Authentication
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default-secret-key";
            var key = Encoding.UTF8.GetBytes(jwtKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "BookMyDoctor",
                        ValidateAudience = true,
                        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "BookMyDoctor",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Swagger with JWT
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() 
                { 
                    Title = "BookMyDoctor API", 
                    Version = "v1" 
                });
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        public static WebApplication UseApiMiddleware(this WebApplication app)
        {
            // Use Serilog
            app.UseSerilogRequestLogging();

            // Custom middleware for request/response logging and error handling
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
                try
                {
                    await next();
                    Console.WriteLine($"Response: {context.Response.StatusCode}");
                }
                catch (BadHttpRequestException ex) when (ex.Message.Contains("JSON"))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid JSON format");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unhandled error: {ex.Message}");
                    throw;
                }
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}