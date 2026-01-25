namespace EmployeeManagementSystem.Api.Models;

/// <summary>
/// Defines the error codes for API responses.
/// </summary>
public enum ApiErrorCode
{
    BadRequest,
    NotFound,
    Unauthorized,
    Conflict,
    InternalError
}

/// <summary>
/// Provides default error messages for each error code.
/// </summary>
public static class ApiErrorMessages
{
    public const string BadRequest = "Invalid request.";
    public const string NotFound = "Resource not found.";
    public const string Unauthorized = "Unauthorized.";
    public const string Conflict = "Conflict occurred.";
    public const string InternalError = "An internal error occurred.";

    /// <summary>
    /// Gets the default message for the specified error code.
    /// </summary>
    public static string GetDefault(ApiErrorCode code)
    {
        return code switch
        {
            ApiErrorCode.BadRequest => BadRequest,
            ApiErrorCode.NotFound => NotFound,
            ApiErrorCode.Unauthorized => Unauthorized,
            ApiErrorCode.Conflict => Conflict,
            ApiErrorCode.InternalError => InternalError,
            _ => BadRequest
        };
    }
}

/// <summary>
/// Represents a standardized API error response.
/// </summary>
public record ApiErrorResponse
{
    /// <summary>
    /// The error code identifying the type of error.
    /// </summary>
    public ApiErrorCode Code { get; init; }

    /// <summary>
    /// The error message describing what went wrong.
    /// </summary>
    public string Message { get; init; }

    private ApiErrorResponse(ApiErrorCode code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Creates an ApiErrorResponse for the specified error code.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">Optional custom message. If null, uses the default message for the code.</param>
    /// <returns>A new ApiErrorResponse instance.</returns>
    public static ApiErrorResponse Create(ApiErrorCode code, string? message = null)
    {
        return new(code, message ?? ApiErrorMessages.GetDefault(code));
    }

    public static ApiErrorResponse BadRequest(string? message = null)
    {
        return Create(ApiErrorCode.BadRequest, message);
    }

    public static ApiErrorResponse NotFound(string? message = null)
    {
        return Create(ApiErrorCode.NotFound, message);
    }

    public static ApiErrorResponse Unauthorized(string? message = null)
    {
        return Create(ApiErrorCode.Unauthorized, message);
    }

    public static ApiErrorResponse Conflict(string? message = null)
    {
        return Create(ApiErrorCode.Conflict, message);
    }

    public static ApiErrorResponse InternalError(string? message = null)
    {
        return Create(ApiErrorCode.InternalError, message);
    }
}
