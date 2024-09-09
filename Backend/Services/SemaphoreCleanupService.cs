
using Backend.Helpers;

namespace Backend.Services;

public class SemaphoreCleanupService : BackgroundService
{
    private readonly SemaphoreManager _semaphoreManager;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5); // Check every minute
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromSeconds(10); // Session timeout period

    public SemaphoreCleanupService(SemaphoreManager semaphoreManager)
    {
        _semaphoreManager = semaphoreManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _semaphoreManager.ReleaseExpiredSemaphores(_sessionTimeout);

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
