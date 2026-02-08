namespace EmployeeManagementSystem.Infrastructure.Messaging.RabbitMQ;

public sealed class RabbitMQSettings
{
    public const string SectionName = "RabbitMQ";

    public string HostName { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string VirtualHost { get; init; } = "ems";
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = string.Empty;
    public string ExchangeName { get; init; } = "ems.events";
    public bool UseSsl { get; init; } = false;
    public int RetryCount { get; init; } = 3;
    public int RetryDelayMilliseconds { get; init; } = 1000;
    public bool Enabled { get; init; } = true;
}
