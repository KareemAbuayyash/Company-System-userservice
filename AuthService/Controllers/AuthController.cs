using Microsoft.AspNetCore.Mvc;
using AuthService.DTOs;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid request data."
                });
            }

            var result = await _authService.AuthenticateAsync(request);
            return Ok(result);
        }

        [HttpPost("validate")]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateUser([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid request data.",
                    Data = false
                });
            }

            var isValid = await _authService.ValidateUserAsync(request.Email, request.Password);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = isValid
            });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
        }
    }
}
