using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Infrastructure.Messaging;
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

        // Register RabbitMQ publisher as concrete type (singleton for connection pooling)
        _ = services.AddSingleton<RabbitMQEventPublisher>();

        // Register decorator as IEventPublisher (scoped to access scoped dependencies)
        _ = services.AddScoped<IEventPublisher>(sp =>
        {
            RabbitMQEventPublisher innerPublisher = sp.GetRequiredService<RabbitMQEventPublisher>();
            IRecentActivityRepository activityRepository = sp.GetRequiredService<IRecentActivityRepository>();
            Microsoft.Extensions.Logging.ILogger<ActivityPersistingEventPublisher> logger =
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ActivityPersistingEventPublisher>>();

            return new ActivityPersistingEventPublisher(innerPublisher, activityRepository, logger);
        });

        return services;
    }
}
