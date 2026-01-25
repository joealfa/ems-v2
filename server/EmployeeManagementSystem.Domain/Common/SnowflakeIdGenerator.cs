namespace EmployeeManagementSystem.Domain.Common;

/// <summary>
/// Generates unique 12-digit IDs using a Snowflake-style algorithm.
/// Structure: 8 digits (timestamp) + 4 digits (sequence/random)
/// This guarantees uniqueness while maintaining roughly chronological ordering.
/// </summary>
public static class SnowflakeIdGenerator
{
    // Custom epoch: January 1, 2024 00:00:00 UTC
    private static readonly DateTime Epoch = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Maximum value for the 4-digit sequence (0-9999)
    private const int MaxSequence = 9999;

    // Lock for thread safety
    private static readonly object Lock = new();

    // Last timestamp used
    private static long _lastTimestamp = -1;

    // Sequence counter for the current millisecond
    private static int _sequence;

    // Random instance for fallback
    private static readonly Random Random = new();

    /// <summary>
    /// Generates a unique 12-digit display ID.
    /// </summary>
    /// <returns>A unique 12-digit long value between 100000000000 and 999999999999.</returns>
    public static long GenerateId()
    {
        lock (Lock)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp == _lastTimestamp)
            {
                // Same millisecond - increment sequence
                _sequence++;

                if (_sequence > MaxSequence)
                {
                    // Sequence overflow - wait for next millisecond
                    timestamp = WaitForNextMillisecond(_lastTimestamp);
                    _sequence = Random.Next(0, 100); // Start with small random offset
                }
            }
            else if (timestamp > _lastTimestamp)
            {
                // New millisecond - reset sequence with small random offset to avoid predictability
                _sequence = Random.Next(0, 100);
            }
            else
            {
                // Clock moved backwards - use random sequence
                _sequence = Random.Next(0, MaxSequence);
            }

            _lastTimestamp = timestamp;

            // Combine: 8-digit timestamp + 4-digit sequence
            // Timestamp: milliseconds since epoch, modulo 100,000,000 (covers ~3.17 years)
            long timestampPart = timestamp % 100_000_000L;
            int sequencePart = _sequence;

            // Ensure timestamp part is at least 10,000,000 to maintain 12 digits total
            // If timestamp is too small (near epoch), add base offset
            if (timestampPart < 10_000_000L)
            {
                timestampPart += 10_000_000L;
            }

            // Final ID: timestampPart (8 digits) * 10000 + sequencePart (4 digits)
            long displayId = (timestampPart * 10_000L) + sequencePart;

            // Ensure we're in the valid 12-digit range
            if (displayId < 100_000_000_000L)
            {
                displayId += 100_000_000_000L;
            }
            else if (displayId > 999_999_999_999L)
            {
                displayId = 100_000_000_000L + (displayId % 900_000_000_000L);
            }

            return displayId;
        }
    }

    /// <summary>
    /// Gets the current timestamp in milliseconds since the custom epoch.
    /// </summary>
    private static long GetCurrentTimestamp()
    {
        return (long)(DateTime.UtcNow - Epoch).TotalMilliseconds;
    }

    /// <summary>
    /// Waits until the next millisecond and returns the new timestamp.
    /// </summary>
    private static long WaitForNextMillisecond(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            Thread.Sleep(0); // Yield to other threads
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}
