using RabbitMQ.Client;
using AuthService.Messaging.Configuration;

namespace AuthService.Messaging.Infrastructure
{
    public interface IRabbitMQConnectionFactory
    {
        IConnection CreateConnection();
        IModel CreateChannel();
        void EnsureQueuesExist(IModel channel);
    }

    public class RabbitMQConnectionFactory : IRabbitMQConnectionFactory, IDisposable
    {
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<RabbitMQConnectionFactory> _logger;
        private IConnection? _connection;
        private readonly object _lock = new object();

        public RabbitMQConnectionFactory(RabbitMQSettings settings, ILogger<RabbitMQConnectionFactory> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public IConnection CreateConnection()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                lock (_lock)
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        var factory = new ConnectionFactory()
                        {
                            HostName = _settings.HostName,
                            Port = _settings.Port,
                            UserName = _settings.UserName,
                            Password = _settings.Password,
                            VirtualHost = _settings.VirtualHost,
                            RequestedHeartbeat = TimeSpan.FromSeconds(60),
                            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                            AutomaticRecoveryEnabled = true
                        };

                        try
                        {
                            _connection = factory.CreateConnection("AuthService");
                            _logger.LogInformation("RabbitMQ connection established successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create RabbitMQ connection");
                            throw;
                        }
                    }
                }
            }
            return _connection;
        }

        public IModel CreateChannel()
        {
            var connection = CreateConnection();
            var channel = connection.CreateModel();
            EnsureQueuesExist(channel);
            return channel;
        }

        public void EnsureQueuesExist(IModel channel)
        {
            try
            {
                // Declare dead letter queue first
                channel.QueueDeclare(
                    queue: _settings.AuthQueues.DeadLetterQueue,
                    durable: _settings.AuthQueues.Durable,
                    exclusive: _settings.AuthQueues.Exclusive,
                    autoDelete: _settings.AuthQueues.AutoDelete,
                    arguments: null);

                // Declare request queue with dead letter exchange
                var requestQueueArgs = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "" },
                    { "x-dead-letter-routing-key", _settings.AuthQueues.DeadLetterQueue },
                    { "x-message-ttl", (int)_settings.RequestTimeout.TotalMilliseconds * 2 }
                };

                channel.QueueDeclare(
                    queue: _settings.AuthQueues.RequestQueue,
                    durable: _settings.AuthQueues.Durable,
                    exclusive: _settings.AuthQueues.Exclusive,
                    autoDelete: _settings.AuthQueues.AutoDelete,
                    arguments: requestQueueArgs);

                _logger.LogInformation("RabbitMQ queues declared successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to declare RabbitMQ queues");
                throw;
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
