using NzolaNet.API.DTOs;

namespace NzolaNet.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        string GenerateJwtToken(int userId, string email, string role);
    }
}
