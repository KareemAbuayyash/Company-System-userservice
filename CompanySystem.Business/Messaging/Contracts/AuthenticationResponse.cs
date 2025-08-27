namespace CompanySystem.Business.Messaging.Contracts
{
    public class AuthenticationResponse
    {
        public string CorrelationId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AuthenticatedUser? User { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AuthenticatedUser
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
    }
}
