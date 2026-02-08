using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Infrastructure.Messaging.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementSystem.Infrastructure.Extensions;

public static class MessagingExtensions
{
    /// <summary>
    /// Adds RabbitMQ messaging services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register RabbitMQ settings
        IConfigurationSection section = configuration.GetSection(RabbitMQSettings.SectionName);
        _ = services.Configure<RabbitMQSettings>(section);

        // Register event publisher as singleton for connection pooling
        _ = services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

        return services;
    }
}
