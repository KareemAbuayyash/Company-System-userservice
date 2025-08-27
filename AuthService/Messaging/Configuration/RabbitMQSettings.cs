namespace AuthService.Messaging.Configuration
{
    public class RabbitMQSettings
    {
        // These properties are expected to be provided via configuration binding (e.g. appsettings.json -> IConfiguration.Bind)
        public required string HostName { get; set; }
        public int Port { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string VirtualHost { get; set; }
        public TimeSpan RequestTimeout { get; set; }
        
        public required AuthQueueSettings AuthQueues { get; set; }
    }

    public class AuthQueueSettings
    {
        public required string RequestQueue { get; set; }
        public required string ResponseQueue { get; set; }
        public required string DeadLetterQueue { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public bool Exclusive { get; set; }
    }
}
