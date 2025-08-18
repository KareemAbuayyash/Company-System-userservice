using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CompanySystem.Business.Interfaces;
using CompanySystem.Business.DTOs;

namespace CompanySystem.Business.Services
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiService> _logger;
        private readonly IConfiguration _configuration;

        public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            
            var authServiceUrl = _configuration["AuthService:BaseUrl"] ?? "http://localhost:5001";
            _httpClient.BaseAddress = new Uri(authServiceUrl);
        }

        public async Task<AuthLoginResponse> LoginAsync(string email, string password)
        {
            try
            {
                var request = new AuthLoginRequest
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<AuthLoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return loginResponse ?? new AuthLoginResponse
                    {
                        Success = false,
                        Message = "Invalid response from authentication service."
                    };
                }
                else
                {
                    _logger.LogWarning("Auth service returned error: {StatusCode} - {Content}", response.StatusCode, responseContent);
                    
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<AuthLoginResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return errorResponse ?? new AuthLoginResponse { Success = false, Message = "Authentication failed." };
                    }
                    catch
                    {
                        return new AuthLoginResponse { Success = false, Message = "Authentication service unavailable." };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error communicating with auth service");
                return new AuthLoginResponse
                {
                    Success = false,
                    Message = "Unable to connect to authentication service. Please try again later."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during authentication");
                return new AuthLoginResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again."
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
