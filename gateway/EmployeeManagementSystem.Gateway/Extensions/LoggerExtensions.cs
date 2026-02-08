namespace EmployeeManagementSystem.Gateway.Extensions;

/// <summary>
/// Extension methods for ILogger that check IsEnabled before logging to avoid CA1873 warnings.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs a message at the specified log level if that level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level to check and log at.</param>
    /// <param name="message">The log message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    public static void LogIfEnabled(
        this ILogger logger,
        LogLevel logLevel,
        string message,
        params object?[] args)
    {
        if (!logger.IsEnabled(logLevel))
        {
            return;
        }

        switch (logLevel)
        {
            case LogLevel.Trace:
                logger.LogTrace(message, args);
                break;
            case LogLevel.Debug:
                logger.LogDebug(message, args);
                break;
            case LogLevel.Information:
                logger.LogInformation(message, args);
                break;
            case LogLevel.Warning:
                logger.LogWarning(message, args);
                break;
            case LogLevel.Error:
                logger.LogError(message, args);
                break;
            case LogLevel.Critical:
                logger.LogCritical(message, args);
                break;
            case LogLevel.None:
                break;
        }
    }

    /// <summary>
    /// Logs an exception with a message at the specified log level if that level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level to check and log at.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    public static void LogIfEnabled(
        this ILogger logger,
        LogLevel logLevel,
        Exception exception,
        string message,
        params object?[] args)
    {
        if (!logger.IsEnabled(logLevel))
        {
            return;
        }

        switch (logLevel)
        {
            case LogLevel.Trace:
                logger.LogTrace(exception, message, args);
                break;
            case LogLevel.Debug:
                logger.LogDebug(exception, message, args);
                break;
            case LogLevel.Information:
                logger.LogInformation(exception, message, args);
                break;
            case LogLevel.Warning:
                logger.LogWarning(exception, message, args);
                break;
            case LogLevel.Error:
                logger.LogError(exception, message, args);
                break;
            case LogLevel.Critical:
                logger.LogCritical(exception, message, args);
                break;
            case LogLevel.None:
                break;
        }
    }
}
