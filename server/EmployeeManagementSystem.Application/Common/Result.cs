namespace EmployeeManagementSystem.Application.Common;

/// <summary>
/// Represents the result of an operation, encapsulating success/failure states.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the type of failure.
    /// </summary>
    public FailureType FailureType { get; }

    private Result(T? value, bool isSuccess, string? error, FailureType failureType)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
        FailureType = failureType;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    public static Result<T> Success(T value)
    {
        return new(value, true, null, FailureType.None);
    }

    /// <summary>
    /// Creates a not found result.
    /// </summary>
    public static Result<T> NotFound(string? message = null)
    {
        return new(default, false, message ?? "Resource not found.", FailureType.NotFound);
    }

    /// <summary>
    /// Creates a bad request result with validation errors.
    /// </summary>
    public static Result<T> BadRequest(string message)
    {
        return new(default, false, message, FailureType.BadRequest);
    }

    /// <summary>
    /// Creates an unauthorized result.
    /// </summary>
    public static Result<T> Unauthorized(string? message = null)
    {
        return new(default, false, message ?? "Unauthorized.", FailureType.Unauthorized);
    }

    /// <summary>
    /// Creates a conflict result (e.g., duplicate resource).
    /// </summary>
    public static Result<T> Conflict(string message)
    {
        return new(default, false, message, FailureType.Conflict);
    }

    /// <summary>
    /// Creates an internal error result.
    /// </summary>
    public static Result<T> InternalError(string message)
    {
        return new(default, false, message, FailureType.InternalError);
    }

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }
}

/// <summary>
/// Represents a result without a value (for void operations).
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the type of failure.
    /// </summary>
    public FailureType FailureType { get; }

    private Result(bool isSuccess, string? error, FailureType failureType)
    {
        IsSuccess = isSuccess;
        Error = error;
        FailureType = failureType;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success()
    {
        return new(true, null, FailureType.None);
    }

    /// <summary>
    /// Creates a not found result.
    /// </summary>
    public static Result NotFound(string? message = null)
    {
        return new(false, message ?? "Resource not found.", FailureType.NotFound);
    }

    /// <summary>
    /// Creates a bad request result.
    /// </summary>
    public static Result BadRequest(string message)
    {
        return new(false, message, FailureType.BadRequest);
    }

    /// <summary>
    /// Creates an unauthorized result.
    /// </summary>
    public static Result Unauthorized(string? message = null)
    {
        return new(false, message ?? "Unauthorized.", FailureType.Unauthorized);
    }

    /// <summary>
    /// Creates a conflict result.
    /// </summary>
    public static Result Conflict(string message)
    {
        return new(false, message, FailureType.Conflict);
    }

    /// <summary>
    /// Creates an internal error result.
    /// </summary>
    public static Result InternalError(string message)
    {
        return new(false, message, FailureType.InternalError);
    }
}

/// <summary>
/// Defines the type of failure for a result.
/// </summary>
public enum FailureType
{
    /// <summary>
    /// No failure - operation was successful.
    /// </summary>
    None = 0,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound = 1,

    /// <summary>
    /// The request was invalid (validation error).
    /// </summary>
    BadRequest = 2,

    /// <summary>
    /// The user is not authorized.
    /// </summary>
    Unauthorized = 3,

    /// <summary>
    /// A conflict occurred (e.g., duplicate resource).
    /// </summary>
    Conflict = 4,

    /// <summary>
    /// An internal server error occurred.
    /// </summary>
    InternalError = 5
}
