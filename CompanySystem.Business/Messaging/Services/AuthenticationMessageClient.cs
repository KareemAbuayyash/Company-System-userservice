using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;
using CompanySystem.Business.Messaging.Contracts;
using CompanySystem.Business.Messaging.Infrastructure;
using CompanySystem.Business.Messaging.Configuration;
using Microsoft.Extensions.Logging;

namespace CompanySystem.Business.Messaging.Services
{
    public interface IAuthenticationMessageClient
    {
        Task<AuthenticationResponse> SendAuthenticationRequestAsync(string email, string password, CancellationToken cancellationToken = default);
    }

    public class AuthenticationMessageClient : IAuthenticationMessageClient, IDisposable
    {
        private readonly IRabbitMQConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<AuthenticationMessageClient> _logger;
        
        private IModel? _channel;
        private EventingBasicConsumer? _consumer;
        private string? _replyQueueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<AuthenticationResponse>> _pendingRequests;

        public AuthenticationMessageClient(
            IRabbitMQConnectionFactory connectionFactory,
            RabbitMQSettings settings,
            ILogger<AuthenticationMessageClient> logger)
        {
            _connectionFactory = connectionFactory;
            _settings = settings;
            _logger = logger;
            _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<AuthenticationResponse>>();
            
            InitializeChannel();
        }

        private void InitializeChannel()
        {
            try
            {
                _channel = _connectionFactory.CreateChannel();
                
                // Create a temporary queue for responses
                _replyQueueName = _channel.QueueDeclare().QueueName;
                
                // Set up consumer for responses
                _consumer = new EventingBasicConsumer(_channel);
                _consumer.Received += OnResponseReceived;
                
                _channel.BasicConsume(
                    queue: _replyQueueName,
                    autoAck: true,
                    consumer: _consumer);

                _logger.LogInformation("Authentication message client initialized with reply queue: {ReplyQueue}", _replyQueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize authentication message client");
                throw;
            }
        }

        public async Task<AuthenticationResponse> SendAuthenticationRequestAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<AuthenticationResponse>();
            
            // Register the pending request
            _pendingRequests[correlationId] = tcs;

            try
            {
                var request = new AuthenticationRequest
                {
                    CorrelationId = correlationId,
                    Email = email,
                    Password = password,
                    Timestamp = DateTime.UtcNow
                };

                var message = JsonConvert.SerializeObject(request);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel?.CreateBasicProperties();
                if (properties != null)
                {
                    properties.CorrelationId = correlationId;
                    properties.ReplyTo = _replyQueueName;
                    properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    properties.Expiration = ((int)_settings.RequestTimeout.TotalMilliseconds).ToString();
                }

                _channel?.BasicPublish(
                    exchange: "",
                    routingKey: _settings.AuthQueues.RequestQueue,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Authentication request sent for correlation ID: {CorrelationId}, Email: {Email}", correlationId, email);

                // Wait for response with timeout
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(_settings.RequestTimeout);

                try
                {
                    var timeoutTask = Task.Delay(_settings.RequestTimeout, timeoutCts.Token);
                    var responseTask = tcs.Task;

                    var completedTask = await Task.WhenAny(responseTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        _logger.LogWarning("Authentication request timed out for correlation ID: {CorrelationId}", correlationId);
                        return new AuthenticationResponse
                        {
                            CorrelationId = correlationId,
                            Success = false,
                            Message = "Authentication request timed out"
                        };
                    }

                    return await responseTask;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Authentication request was cancelled for correlation ID: {CorrelationId}", correlationId);
                    return new AuthenticationResponse
                    {
                        CorrelationId = correlationId,
                        Success = false,
                        Message = "Authentication request was cancelled"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending authentication request for correlation ID: {CorrelationId}", correlationId);
                return new AuthenticationResponse
                {
                    CorrelationId = correlationId,
                    Success = false,
                    Message = "An error occurred while sending authentication request"
                };
            }
            finally
            {
                // Clean up pending request
                _pendingRequests.TryRemove(correlationId, out _);
            }
        }

        private void OnResponseReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var response = JsonConvert.DeserializeObject<AuthenticationResponse>(message);

                if (response == null)
                {
                    _logger.LogWarning("Received null authentication response");
                    return;
                }

                var correlationId = response.CorrelationId;
                _logger.LogInformation("Received authentication response for correlation ID: {CorrelationId}, Success: {Success}", 
                    correlationId, response.Success);

                if (_pendingRequests.TryRemove(correlationId, out var tcs))
                {
                    tcs.SetResult(response);
                }
                else
                {
                    _logger.LogWarning("No pending request found for correlation ID: {CorrelationId}", correlationId);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize authentication response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing authentication response");
            }
        }

        public void Dispose()
        {
            // Complete any pending requests with error
            foreach (var kvp in _pendingRequests)
            {
                if (kvp.Value.Task.IsCompleted == false)
                {
                    kvp.Value.SetResult(new AuthenticationResponse
                    {
                        CorrelationId = kvp.Key,
                        Success = false,
                        Message = "Service is shutting down"
                    });
                }
            }

            _pendingRequests.Clear();
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}
