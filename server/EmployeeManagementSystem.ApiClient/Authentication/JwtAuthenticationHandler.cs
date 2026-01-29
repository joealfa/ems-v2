namespace EmployeeManagementSystem.ApiClient.Authentication;

/// <summary>
/// HTTP message handler that adds JWT authentication to outgoing requests.
/// Supports both static tokens and dynamic token provider functions.
/// </summary>
public class JwtAuthenticationHandler : DelegatingHandler
{
    private readonly Func<CancellationToken, Task<string?>>? _tokenProvider;
    private readonly string? _staticToken;

    /// <summary>
    /// Creates a JWT authentication handler with a static token.
    /// </summary>
    /// <param name="token">The JWT token to use for all requests.</param>
    public JwtAuthenticationHandler(string token)
    {
        _staticToken = token ?? throw new ArgumentNullException(nameof(token));
    }

    /// <summary>
    /// Creates a JWT authentication handler with a dynamic token provider.
    /// </summary>
    /// <param name="tokenProvider">A function that returns the JWT token for each request.</param>
    public JwtAuthenticationHandler(Func<CancellationToken, Task<string?>> tokenProvider)
    {
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? token = _staticToken ?? await _tokenProvider!(cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
