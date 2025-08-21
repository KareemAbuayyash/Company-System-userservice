using CompanySystem.Business.DTOs;
using CompanySystem.Business.Interfaces;
using CompanySystem.Business.Messaging.Services;
using Microsoft.Extensions.Logging;

namespace CompanySystem.Business.Services
{
    public class RabbitMQAuthApiService : IAuthApiService
    {
        private readonly IAuthenticationMessageClient _messageClient;
        private readonly ILogger<RabbitMQAuthApiService> _logger;

        public RabbitMQAuthApiService(
            IAuthenticationMessageClient messageClient,
            ILogger<RabbitMQAuthApiService> logger)
        {
            _messageClient = messageClient;
            _logger = logger;
        }

        public async Task<AuthLoginResponse> LoginAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation("Sending authentication request via RabbitMQ for email: {Email}", email);

                var response = await _messageClient.SendAuthenticationRequestAsync(email, password);

                var result = new AuthLoginResponse
                {
                    Success = response.Success,
                    Message = response.Message,
                    User = response.User != null ? new AuthUserInfo
                    {
                        UserId = response.User.UserId,
                        Email = response.User.Email,
                        FirstName = response.User.FirstName,
                        LastName = response.User.LastName,
                        Role = response.User.Role,
                        LastLoginDate = response.User.LastLoginDate
                    } : null
                };

                _logger.LogInformation("Authentication completed via RabbitMQ for email: {Email}, Success: {Success}", 
                    email, result.Success);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RabbitMQ authentication for email: {Email}", email);
                return new AuthLoginResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication. Please try again later."
                };
            }
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var result = await LoginAsync(email, password);
            return result.Success;
        }
    }
}
