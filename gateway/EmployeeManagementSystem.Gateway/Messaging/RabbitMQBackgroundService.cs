using EmployeeManagementSystem.Gateway.Extensions;

namespace EmployeeManagementSystem.Gateway.Messaging;

/// <summary>
/// Background service that runs the RabbitMQ event consumer.
/// </summary>
public sealed class RabbitMQBackgroundService(
    RabbitMQEventConsumer consumer,
    ILogger<RabbitMQBackgroundService> logger) : BackgroundService
{
    private readonly RabbitMQEventConsumer _consumer = consumer;
    private readonly ILogger<RabbitMQBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogIfEnabled(LogLevel.Information, "RabbitMQ Background Service starting");

        try
        {
            await _consumer.StartAsync(stoppingToken);

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogIfEnabled(LogLevel.Information, "RabbitMQ Background Service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogIfEnabled(LogLevel.Error, ex, "Fatal error in RabbitMQ Background Service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogIfEnabled(LogLevel.Information, "RabbitMQ Background Service stopping");
        await _consumer.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
