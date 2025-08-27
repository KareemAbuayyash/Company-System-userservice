using Microsoft.AspNetCore.Mvc;
using CompanySystem.Business.Interfaces;

namespace CompanySystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IAuthApiService _authApiService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IAuthApiService authApiService, ILogger<HealthController> logger)
        {
            _authApiService = authApiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Status = "Healthy", Service = "CompanySystem.Web", Timestamp = DateTime.UtcNow });
        }

        [HttpGet("auth")]
        public async Task<IActionResult> TestAuth()
        {
            try
            {
                // Test with a known credential - adjust as needed
                var result = await _authApiService.LoginAsync("admin@company.com", "password123");
                
                return Ok(new 
                { 
                    Status = "Auth service communication successful",
                    AuthResult = result.Success,
                    Message = result.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auth service health check failed");
                return StatusCode(500, new 
                { 
                    Status = "Auth service communication failed", 
                    Error = ex.Message, 
                    Timestamp = DateTime.UtcNow 
                });
            }
        }
    }
}
