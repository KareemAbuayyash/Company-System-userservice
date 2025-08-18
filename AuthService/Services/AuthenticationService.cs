using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;

namespace AuthService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthDbContext _context;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(AuthDbContext context, ILogger<AuthenticationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                if (!VerifyPassword(request.Password, user.Password))
                {
                    _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                await UpdateLastLoginAsync(user.UserId);

                _logger.LogInformation("Successful login for user: {Email}", request.Email);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    User = new UserInfo
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        LastLoginDate = DateTime.UtcNow
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for email: {Email}", request.Email);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication."
                };
            }
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var result = await AuthenticateAsync(new LoginRequest { Email = email, Password = password });
            return result.Success;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                return false;
            }
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.LastLoginDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for user: {UserId}", userId);
            }
        }
    }
}
