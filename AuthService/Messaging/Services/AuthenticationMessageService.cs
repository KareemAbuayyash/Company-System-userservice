using AuthService.Messaging.Handlers;

namespace AuthService.Messaging.Services
{
    public class AuthenticationMessageService : BackgroundService
    {
        private readonly IAuthenticationMessageHandler _messageHandler;
        private readonly ILogger<AuthenticationMessageService> _logger;

        public AuthenticationMessageService(
            IAuthenticationMessageHandler messageHandler,
            ILogger<AuthenticationMessageService> logger)
        {
            _messageHandler = messageHandler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Authentication Message Service starting");

            try
            {
                await _messageHandler.StartListeningAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Authentication Message Service is stopping");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication Message Service encountered an error");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Authentication Message Service is stopping");
            await _messageHandler.StopListeningAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
