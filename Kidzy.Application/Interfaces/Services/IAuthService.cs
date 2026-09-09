using Kidzy.Application.DTOs.Auth;

namespace Kidzy.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(
            RegisterDto registerDto);

        Task<AuthResponseDto?> LoginAsync(
            LoginDto loginDto);
    }
}