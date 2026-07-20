using Identity.infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.infrastructure.Services
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RefreshTokenCleanupService> _logger;

        public RefreshTokenCleanupService(IServiceProvider serviceProvider, ILogger<RefreshTokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run once shortly after startup, then on a fixed interval.
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupAsync(stoppingToken);

                try
                {
                    await Task.Delay(Interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // shutting down
                }
            }
        }

        private async Task CleanupAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

                var cutoff = DateTime.UtcNow;
                var deletable = await dbContext.RefreshTokenss
                    .Where(t => t.IsRevoked || t.ExpirationDate < cutoff)
                    .ToListAsync(cancellationToken);

                if (deletable.Count == 0)
                    return;

                dbContext.RefreshTokenss.RemoveRange(deletable);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Refresh token cleanup removed {Count} expired/revoked token(s).", deletable.Count);
            }
            catch (Exception ex)
            {
                // Never let a cleanup failure take the host down.
                _logger.LogError(ex, "Refresh token cleanup run failed.");
            }
        }
    }
}
