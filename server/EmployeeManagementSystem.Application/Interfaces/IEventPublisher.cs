using EmployeeManagementSystem.Application.Events;
using EmployeeManagementSystem.Domain.Events;

namespace EmployeeManagementSystem.Application.Interfaces;

public interface IEventPublisher
{
    /// <summary>
    /// Publish a domain event to the message broker
    /// </summary>
    Task PublishAsync<TEvent>(
        TEvent domainEvent,
        string? userId = null,
        string? correlationId = null,
        EventMetadata? metadata = null,
        CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;

    /// <summary>
    /// Publish multiple domain events in batch
    /// </summary>
    Task PublishBatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        string? userId = null,
        string? correlationId = null,
        EventMetadata? metadata = null,
        CancellationToken cancellationToken = default);
}
