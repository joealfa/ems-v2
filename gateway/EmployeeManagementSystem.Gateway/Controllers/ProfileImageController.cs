using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EmployeeManagementSystem.Gateway.Controllers;

/// <summary>
/// REST controller for proxying profile image requests to the backend API.
/// GraphQL doesn't handle binary file streaming well, so we use REST for downloads.
/// </summary>
[ApiController]
[Route("api/persons/{personDisplayId:long}/profile-image")]
public class ProfileImageController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<ProfileImageController> logger) : ControllerBase
{
    /// <summary>
    /// Get profile image for a person (proxied from backend API)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProfileImage(long personDisplayId, CancellationToken ct)
    {
        try
        {
            // Create HTTP client and forward the request to the backend API
            using HttpClient client = httpClientFactory.CreateClient();
            string apiBaseUrl = configuration["ApiClient:BaseUrl"] ?? "https://localhost:7166";
            string requestUrl = $"{apiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents/profile-image";

            // Forward the authorization header if present
            using HttpRequestMessage request = new(HttpMethod.Get, requestUrl);
            if (Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
            {
                request.Headers.Add("Authorization", authHeader.ToString());
            }

            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                logger.LogWarning("Failed to fetch profile image for person {PersonDisplayId}: {StatusCode}",
                    personDisplayId, response.StatusCode);
                return StatusCode((int)response.StatusCode);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(ct);
            string contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";

            return File(stream, contentType);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error fetching profile image for person {PersonDisplayId}", personDisplayId);
            return StatusCode(502, "Error connecting to backend service");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving profile image for person {PersonDisplayId}", personDisplayId);
            return StatusCode(500, "Error retrieving profile image");
        }
    }
}
