using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Business.Messaging.Contracts
{
    public class AuthenticationRequest
    {
        [Required]
        public string CorrelationId { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
