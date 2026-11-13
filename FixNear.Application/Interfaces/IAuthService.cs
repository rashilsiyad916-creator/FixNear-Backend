using FixNear.Application.DTOs.Auth;

namespace FixNear.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserInfoDto?> GetMeAsync(int userId);
    }
}
