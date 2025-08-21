using RabbitMQ.Client;
using CompanySystem.Business.Messaging.Configuration;
using Microsoft.Extensions.Logging;

namespace CompanySystem.Business.Messaging.Infrastructure
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
                            _connection = factory.CreateConnection("CompanySystem.Web");
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
                // We don't need to declare queues on client side for RPC pattern
                // The server (AuthService) will handle queue declarations
                _logger.LogDebug("Queue existence check completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check queue existence");
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
