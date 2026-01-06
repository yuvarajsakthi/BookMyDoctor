using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration config, IUnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public string GenerateToken(UserDto userDto)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userDto.UserId.ToString()),
                new(ClaimTypes.Email, userDto.Email),
                new(ClaimTypes.Role, userDto.UserRole.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default-secret-key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "BookMyDoctor",
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "BookMyDoctor",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }
 
            if (!string.IsNullOrEmpty(password) && !VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                UserRole = user.UserRole
            };

            return GenerateToken(userDto);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
