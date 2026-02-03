using EmployeeManagementSystem.ApiClient.Generated;
using HotChocolate;

namespace EmployeeManagementSystem.Gateway.Errors;

public sealed class ApiExceptionErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is not ApiException apiException)
        {
            return error;
        }

        // NSwag throws ApiException for non-2xx responses.
        // If the backend says 401/403, expose that as a proper GraphQL auth error instead of
        // HotChocolate returning a generic "Unexpected Execution Error".
        return apiException.StatusCode switch
        {
            401 => error.WithMessage("Unauthorized").WithCode("UNAUTHENTICATED"),
            403 => error.WithMessage("Forbidden").WithCode("FORBIDDEN"),
            _ => error.WithMessage($"Backend request failed ({apiException.StatusCode}).")
        };
    }
}
