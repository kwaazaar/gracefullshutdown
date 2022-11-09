using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SlowShutdownWorker
{
    internal class BackgroundWorker : IHostedService
    {
        private readonly TimeSpan _shutdownDuration;
        private readonly ILogger<BackgroundWorker> _logger;

        public BackgroundWorker(ILogger<BackgroundWorker> logger, TimeSpan shutdownDuration)
        {
            _shutdownDuration = shutdownDuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started with shutdown duration configured as {shutdownDuration}", _shutdownDuration);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down using shutdown duration of {shutdownDuration}", _shutdownDuration);
            await Task.Delay(_shutdownDuration); // Ignore cancellationtoken, since we really need to wait _shutdownDuration
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Shutdown completed, but took longer than allowed by cancellationToken");
            }
            else
            {
                _logger.LogInformation("Shutdown completed, which was allowed by cancellationToken (not cancelled yet)");
            }
        }
    }
}
