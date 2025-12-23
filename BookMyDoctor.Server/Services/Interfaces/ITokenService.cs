using BookMyDoctor.Server.DTOs;
namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(UserDto userDto);
        Task<string?> AuthenticateAsync(string email, string password);
    }
}
