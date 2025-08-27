using Microsoft.AspNetCore.Mvc;
using CompanySystem.Business.Interfaces;

namespace CompanySystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RabbitMQTestController : ControllerBase
    {
        private readonly IAuthApiService _authApiService;
        private readonly ILogger<RabbitMQTestController> _logger;

        public RabbitMQTestController(IAuthApiService authApiService, ILogger<RabbitMQTestController> logger)
        {
            _authApiService = authApiService;
            _logger = logger;
        }

        [HttpPost("test-login")]
        public async Task<IActionResult> TestLogin([FromBody] TestLoginRequest request)
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation("🧪 Starting RabbitMQ test login for: {Email}", request.Email);

            try
            {
                var result = await _authApiService.LoginAsync(request.Email, request.Password);
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                var response = new
                {
                    Success = result.Success,
                    Message = result.Message,
                    User = result.User,
                    TestInfo = new
                    {
                        RequestTime = startTime,
                        ResponseTime = endTime,
                        Duration = $"{duration.TotalMilliseconds:F2}ms",
                        Transport = "RabbitMQ",
                        TestStatus = result.Success ? "✅ PASSED" : "❌ FAILED"
                    }
                };

                _logger.LogInformation("🧪 RabbitMQ test completed in {Duration}ms - {Status}", 
                    duration.TotalMilliseconds, result.Success ? "SUCCESS" : "FAILED");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🧪 RabbitMQ test failed for: {Email}", request.Email);
                return StatusCode(500, new 
                { 
                    Success = false,
                    Message = "Test failed with exception",
                    Error = ex.Message,
                    TestStatus = "❌ ERROR"
                });
            }
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                Status = "✅ RabbitMQ Integration Active",
                Transport = "RabbitMQ Message Queue",
                Features = new[]
                {
                    "✅ Request/Response Pattern",
                    "✅ Correlation ID Tracking", 
                    "✅ Async Processing",
                    "✅ Error Handling",
                    "✅ Connection Pooling"
                },
                Endpoints = new
                {
                    AuthService = "http://localhost:5001",
                    WebService = "http://localhost:5003",
                    RabbitMQManagement = "http://localhost:15672"
                },
                LastUpdated = DateTime.UtcNow
            });
        }
    }

    public class TestLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
