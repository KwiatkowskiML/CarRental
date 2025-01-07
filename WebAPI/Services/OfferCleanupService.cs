using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.Services;

public class OfferCleanupService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OfferCleanupService> _logger;
    private Timer? _timer;

    public OfferCleanupService(ILogger<OfferCleanupService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OfferCleanupService running.");
        
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        _logger.LogInformation("OfferCleanupService is working.");
        
        using var scope = _serviceProvider.CreateScope(); // Create a new DI scope
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            await unitOfWork.OffersRepository.DeleteExpiredOffersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OfferCleanupService");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}