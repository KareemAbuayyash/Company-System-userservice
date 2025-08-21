using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using AuthService.Messaging.Contracts;
using AuthService.Messaging.Infrastructure;
using AuthService.Messaging.Configuration;
using AuthService.Services;
using AuthService.DTOs;

namespace AuthService.Messaging.Handlers
{
    public interface IAuthenticationMessageHandler
    {
        Task StartListeningAsync(CancellationToken cancellationToken);
        Task StopListeningAsync();
    }

    public class AuthenticationMessageHandler : IAuthenticationMessageHandler, IDisposable
    {
        private readonly IRabbitMQConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<AuthenticationMessageHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        private IModel? _channel;
        private EventingBasicConsumer? _consumer;
        private string? _consumerTag;

        public AuthenticationMessageHandler(
            IRabbitMQConnectionFactory connectionFactory,
            RabbitMQSettings settings,
            ILogger<AuthenticationMessageHandler> logger,
            IServiceProvider serviceProvider)
        {
            _connectionFactory = connectionFactory;
            _settings = settings;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            try
            {
                _channel = _connectionFactory.CreateChannel();
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                _consumer = new EventingBasicConsumer(_channel);
                _consumer.Received += OnMessageReceived;

                _consumerTag = _channel.BasicConsume(
                    queue: _settings.AuthQueues.RequestQueue,
                    autoAck: false,
                    consumer: _consumer);

                _logger.LogInformation("Authentication message handler started listening on queue: {Queue}", 
                    _settings.AuthQueues.RequestQueue);

                // Keep the handler running
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Authentication message handler stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in authentication message handler");
                throw;
            }
        }

        public Task StopListeningAsync()
        {
            try
            {
                if (_channel != null && !string.IsNullOrEmpty(_consumerTag))
                {
                    _channel.BasicCancel(_consumerTag);
                }

                _channel?.Close();
                _logger.LogInformation("Authentication message handler stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping authentication message handler");
            }
            
            return Task.CompletedTask;
        }

        private async void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var correlationId = string.Empty;
            
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var request = JsonConvert.DeserializeObject<AuthenticationRequest>(message);

                if (request == null)
                {
                    _logger.LogWarning("Received null authentication request");
                    _channel?.BasicNack(e.DeliveryTag, false, false);
                    return;
                }

                correlationId = request.CorrelationId;
                _logger.LogInformation("Processing authentication request for correlation ID: {CorrelationId}", correlationId);

                // Process authentication using scoped service
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

                var loginRequest = new LoginRequest
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var authResult = await authService.AuthenticateAsync(loginRequest);

                // Create response
                var response = new AuthenticationResponse
                {
                    CorrelationId = correlationId,
                    Success = authResult.Success,
                    Message = authResult.Message,
                    User = authResult.User != null ? new AuthenticatedUser
                    {
                        UserId = authResult.User.UserId,
                        Email = authResult.User.Email,
                        FirstName = authResult.User.FirstName,
                        LastName = authResult.User.LastName,
                        Role = authResult.User.Role,
                        LastLoginDate = authResult.User.LastLoginDate
                    } : null
                };

                // Send response
                await SendResponseAsync(response, e.BasicProperties.ReplyTo, e.BasicProperties.CorrelationId);

                // Acknowledge message
                _channel?.BasicAck(e.DeliveryTag, false);
                
                _logger.LogInformation("Authentication request processed successfully for correlation ID: {CorrelationId}, Success: {Success}", 
                    correlationId, response.Success);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize authentication request for correlation ID: {CorrelationId}", correlationId);
                _channel?.BasicNack(e.DeliveryTag, false, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing authentication request for correlation ID: {CorrelationId}", correlationId);
                
                // Send error response if possible
                if (!string.IsNullOrEmpty(correlationId) && !string.IsNullOrEmpty(e.BasicProperties.ReplyTo))
                {
                    var errorResponse = new AuthenticationResponse
                    {
                        CorrelationId = correlationId,
                        Success = false,
                        Message = "An error occurred during authentication processing"
                    };

                    try
                    {
                        await SendResponseAsync(errorResponse, e.BasicProperties.ReplyTo, e.BasicProperties.CorrelationId);
                    }
                    catch (Exception sendEx)
                    {
                        _logger.LogError(sendEx, "Failed to send error response for correlation ID: {CorrelationId}", correlationId);
                    }
                }

                _channel?.BasicNack(e.DeliveryTag, false, false);
            }
        }

        private Task SendResponseAsync(AuthenticationResponse response, string replyTo, string correlationId)
        {
            if (string.IsNullOrEmpty(replyTo))
            {
                _logger.LogWarning("No reply-to queue specified for correlation ID: {CorrelationId}", correlationId);
                return Task.CompletedTask;
            }

            try
            {
                var responseMessage = JsonConvert.SerializeObject(response);
                var responseBody = Encoding.UTF8.GetBytes(responseMessage);

                var properties = _channel?.CreateBasicProperties();
                if (properties != null)
                {
                    properties.CorrelationId = correlationId;
                    properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                }

                _channel?.BasicPublish(
                    exchange: "",
                    routingKey: replyTo,
                    basicProperties: properties,
                    body: responseBody);

                _logger.LogDebug("Response sent to queue: {ReplyTo} for correlation ID: {CorrelationId}", replyTo, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send response for correlation ID: {CorrelationId}", correlationId);
                throw;
            }
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}
