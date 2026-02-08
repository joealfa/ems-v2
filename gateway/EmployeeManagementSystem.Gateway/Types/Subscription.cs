using EmployeeManagementSystem.Gateway.Services;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace EmployeeManagementSystem.Gateway.Types;

/// <summary>
/// GraphQL subscription type for real-time activity events.
/// </summary>
public class Subscription
{
    /// <summary>
    /// Subscribes to real-time activity events.
    /// When a client first subscribes, they receive all buffered recent events,
    /// followed by new events as they occur.
    /// </summary>
    /// <param name="buffer">The activity event buffer containing recent events.</param>
    /// <param name="receiver">The topic event receiver for subscribing to events.</param>
    /// <param name="cancellationToken">Cancellation token for the subscription.</param>
    /// <returns>An async enumerable stream of activity events.</returns>
    [Subscribe(With = nameof(SubscribeToActivityEventsAsync))]
    public ActivityEventDto SubscribeToActivityEvents(
        [EventMessage] ActivityEventDto activityEvent) => activityEvent;

    /// <summary>
    /// Subscription setup: sends buffered events first, then subscribes to new events.
    /// </summary>
    public async ValueTask<ISourceStream<ActivityEventDto>> SubscribeToActivityEventsAsync(
        [Service] ActivityEventBuffer buffer,
        [Service] ITopicEventReceiver receiver,
        CancellationToken cancellationToken)
    {
        // Get buffered events
        var bufferedEvents = buffer.GetRecentEvents();

        // Subscribe to new events
        var stream = await receiver.SubscribeAsync<ActivityEventDto>("ActivityEvent", cancellationToken);

        // Wrap the stream to prepend buffered events
        return new BufferedSourceStream(bufferedEvents, stream);
    }

    /// <summary>
    /// Custom source stream that yields buffered events before live events.
    /// </summary>
    private class BufferedSourceStream : ISourceStream<ActivityEventDto>
    {
        private readonly IReadOnlyList<ActivityEventDto> _bufferedEvents;
        private readonly ISourceStream<ActivityEventDto> _liveStream;

        public BufferedSourceStream(
            IReadOnlyList<ActivityEventDto> bufferedEvents,
            ISourceStream<ActivityEventDto> liveStream)
        {
            _bufferedEvents = bufferedEvents;
            _liveStream = liveStream;
        }

        IAsyncEnumerable<ActivityEventDto> ISourceStream<ActivityEventDto>.ReadEventsAsync()
        {
            return ReadEventsInternalAsync();
        }

        IAsyncEnumerable<object?> ISourceStream.ReadEventsAsync()
        {
            return ReadEventsInternalAsObjectAsync();
        }

        private async IAsyncEnumerable<ActivityEventDto> ReadEventsInternalAsync()
        {
            // First, yield all buffered events
            foreach (var bufferedEvent in _bufferedEvents)
            {
                yield return bufferedEvent;
            }

            // Then, yield live events from the stream
            await foreach (var liveEvent in _liveStream.ReadEventsAsync())
            {
                yield return liveEvent;
            }
        }

        private async IAsyncEnumerable<object?> ReadEventsInternalAsObjectAsync()
        {
            // First, yield all buffered events
            foreach (var bufferedEvent in _bufferedEvents)
            {
                yield return bufferedEvent;
            }

            // Then, yield live events from the stream as base interface
            await foreach (var liveEvent in ((ISourceStream)_liveStream).ReadEventsAsync())
            {
                yield return liveEvent;
            }
        }

        public ValueTask DisposeAsync() => _liveStream.DisposeAsync();
    }
}
