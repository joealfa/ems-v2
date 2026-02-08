using System.Collections.Concurrent;
using EmployeeManagementSystem.Gateway.Types;

namespace EmployeeManagementSystem.Gateway.Services;

/// <summary>
/// Thread-safe in-memory buffer for recent activity events.
/// Maintains a circular buffer of the last N events.
/// </summary>
public class ActivityEventBuffer
{
    private readonly ConcurrentQueue<ActivityEventDto> _events = new();
    private readonly int _maxCapacity;
    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityEventBuffer"/> class.
    /// </summary>
    /// <param name="maxCapacity">Maximum number of events to keep in buffer (default: 50).</param>
    public ActivityEventBuffer(int maxCapacity = 50)
    {
        _maxCapacity = maxCapacity;
    }

    /// <summary>
    /// Adds a new event to the buffer. If capacity is exceeded, oldest events are removed.
    /// </summary>
    /// <param name="activityEvent">The activity event to add.</param>
    public void AddEvent(ActivityEventDto activityEvent)
    {
        lock (_lock)
        {
            _events.Enqueue(activityEvent);

            // Remove oldest events if capacity exceeded
            while (_events.Count > _maxCapacity)
            {
                _events.TryDequeue(out _);
            }
        }
    }

    /// <summary>
    /// Gets all recent events in chronological order (oldest first).
    /// </summary>
    /// <returns>A list of recent activity events.</returns>
    public IReadOnlyList<ActivityEventDto> GetRecentEvents()
    {
        lock (_lock)
        {
            return _events.ToList();
        }
    }

    /// <summary>
    /// Gets the current count of events in the buffer.
    /// </summary>
    public int Count => _events.Count;
}
