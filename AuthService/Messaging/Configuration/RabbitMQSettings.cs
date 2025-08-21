namespace AuthService.Messaging.Configuration
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
        
        public AuthQueueSettings AuthQueues { get; set; } = new();
    }

    public class AuthQueueSettings
    {
        public string RequestQueue { get; set; } = "auth.login.request";
        public string ResponseQueue { get; set; } = "auth.login.response";
        public string DeadLetterQueue { get; set; } = "auth.login.dlq";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public bool Exclusive { get; set; } = false;
    }
}
