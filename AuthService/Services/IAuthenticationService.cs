using AuthService.DTOs;

namespace AuthService.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> AuthenticateAsync(LoginRequest request);
        Task<bool> ValidateUserAsync(string email, string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task UpdateLastLoginAsync(int userId);
    }
}
