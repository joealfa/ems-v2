using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace EmployeeManagementSystem.Gateway.Controllers;

/// <summary>
/// Proxy controller for authentication operations.
/// These endpoints forward requests to the API server.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    private string ApiBaseUrl => _configuration["ApiClient:BaseUrl"] ?? "https://localhost:7166";

    /// <summary>
    /// Proxy Google login to the API server
    /// </summary>
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] JsonElement body, CancellationToken ct)
    {
        return await ProxyJsonRequest("POST", $"{ApiBaseUrl}/api/v1/Auth/google", body, ct);
    }

    /// <summary>
    /// Proxy Google token login to the API server
    /// </summary>
    [HttpPost("google/token")]
    public async Task<IActionResult> GoogleTokenLogin([FromBody] JsonElement body, CancellationToken ct)
    {
        return await ProxyJsonRequest("POST", $"{ApiBaseUrl}/api/v1/Auth/google/token", body, ct);
    }

    /// <summary>
    /// Proxy token refresh to the API server
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] JsonElement body, CancellationToken ct)
    {
        return await ProxyJsonRequest("POST", $"{ApiBaseUrl}/api/v1/Auth/refresh", body, ct);
    }

    /// <summary>
    /// Proxy token revoke/logout to the API server
    /// </summary>
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken([FromBody] JsonElement body, CancellationToken ct)
    {
        return await ProxyJsonRequest("POST", $"{ApiBaseUrl}/api/v1/Auth/revoke", body, ct);
    }

    /// <summary>
    /// Proxy get current user to the API server
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        HttpClient client = CreateClient();
        HttpResponseMessage response = await client.GetAsync($"{ApiBaseUrl}/api/v1/Auth/me", ct);
        return await ProxyResponse(response, ct);
    }

    private HttpClient CreateClient()
    {
        HttpClient client = _httpClientFactory.CreateClient("EmsApiClient");

        // Forward the Authorization header from the incoming request
        if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues authHeader))
        {
            _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authHeader.ToString());
        }

        return client;
    }

    private async Task<IActionResult> ProxyJsonRequest(string method, string url, JsonElement body, CancellationToken ct)
    {
        HttpClient client = CreateClient();
        StringContent jsonContent = new(body.GetRawText(), Encoding.UTF8, "application/json");

        // Forward cookies for refresh token
        if (Request.Headers.TryGetValue("Cookie", out Microsoft.Extensions.Primitives.StringValues cookieHeader))
        {
            _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", cookieHeader.ToString());
        }

        HttpResponseMessage response = method == "POST"
            ? await client.PostAsync(url, jsonContent, ct)
            : method == "PUT"
                ? await client.PutAsync(url, jsonContent, ct)
                : throw new ArgumentException($"Unsupported HTTP method: {method}");
        return await ProxyResponse(response, ct);
    }

    private async Task<IActionResult> ProxyResponse(HttpResponseMessage response, CancellationToken ct)
    {
        string content = await response.Content.ReadAsStringAsync(ct);

        // Forward Set-Cookie headers (for refresh token HttpOnly cookie)
        if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? setCookieValues))
        {
            foreach (string cookie in setCookieValues)
            {
                Response.Headers.Append("Set-Cookie", cookie);
            }
        }

        return response.IsSuccessStatusCode
            ? Content(content, response.Content.Headers.ContentType?.ToString() ?? "application/json")
            : StatusCode((int)response.StatusCode, content);
    }
}
