namespace EmployeeManagementSystem.Gateway.Messaging;

/// <summary>
/// Background service that runs the RabbitMQ event consumer.
/// </summary>
public sealed class RabbitMQBackgroundService : BackgroundService
{
    private readonly RabbitMQEventConsumer _consumer;
    private readonly ILogger<RabbitMQBackgroundService> _logger;

    public RabbitMQBackgroundService(
        RabbitMQEventConsumer consumer,
        ILogger<RabbitMQBackgroundService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ Background Service starting");

        try
        {
            await _consumer.StartAsync(stoppingToken);

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ Background Service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in RabbitMQ Background Service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Background Service stopping");
        await _consumer.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
