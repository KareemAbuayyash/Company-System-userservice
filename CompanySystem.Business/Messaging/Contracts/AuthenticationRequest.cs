using System.ComponentModel.DataAnnotations;

namespace CompanySystem.Business.Messaging.Contracts
{
    public class AuthenticationRequest
    {
        [Required]
        public string CorrelationId { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}
