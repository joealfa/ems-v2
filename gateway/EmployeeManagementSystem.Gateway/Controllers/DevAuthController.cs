#if DEBUG
using EmployeeManagementSystem.Gateway.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Gateway.Controllers;

/// <summary>
/// Development-only controller that proxies dev auth tokens from the backend
/// and sets them as HttpOnly cookies. Only available in DEBUG builds.
/// </summary>
[ApiController]
[Route("api/dev/auth")]
public class DevAuthController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<DevAuthController> logger) : ControllerBase
{
    [HttpPost("token")]
    public async Task<IActionResult> GetDevToken([FromBody] DevTokenRequest? request, CancellationToken ct)
    {
        string apiBaseUrl = configuration["ApiClient:BaseUrl"] ?? "https://localhost:7166";

        try
        {
            using HttpClient client = httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsJsonAsync(
                $"{apiBaseUrl}/api/v1/dev/devauth/token",
                request ?? new DevTokenRequest(),
                ct);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync(ct);
                logger.LogIfEnabled(LogLevel.Warning, "Dev auth token request failed: {StatusCode} {Body}",
                    response.StatusCode, errorBody);
                return StatusCode((int)response.StatusCode, errorBody);
            }

            DevTokenResponse? data = await response.Content.ReadFromJsonAsync<DevTokenResponse>(ct);
            if (data is null)
            {
                return StatusCode(500, "Failed to parse dev token response");
            }

            // Set access token as HttpOnly cookie (same as production auth flow)
            // Secure=true + SameSite=Lax works in dev since frontend uses HTTPS
            Response.Cookies.Append("accessToken", data.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.Parse(data.ExpiresAt)
            });

            logger.LogIfEnabled(LogLevel.Information, "Dev auth token set as HttpOnly cookie for {Email}", data.Email);

            // Return user info (but NOT the token — it's in the cookie)
            return Ok(new
            {
                data.UserId,
                data.Email,
                data.Name,
                data.ExpiresAt
            });
        }
        catch (HttpRequestException ex)
        {
            logger.LogIfEnabled(LogLevel.Error, ex, "Failed to connect to backend dev auth endpoint");
            return StatusCode(502, "Error connecting to backend service");
        }
    }

    public class DevTokenRequest
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

    private class DevTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string ExpiresAt { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
#endif
