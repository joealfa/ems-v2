using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EmployeeManagementSystem.Gateway.Controllers;

/// <summary>
/// Proxy controller for document operations that require file handling.
/// These endpoints forward requests to the API server with proper authentication.
/// </summary>
[ApiController]
[Route("api/persons/{personDisplayId:long}/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public DocumentsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    private string ApiBaseUrl => _configuration["ApiClient:BaseUrl"] ?? "https://localhost:7166";

    /// <summary>
    /// Proxy document upload to the API server
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument(
        long personDisplayId,
        [FromForm] IFormFile file,
        [FromForm] string? description,
        CancellationToken ct)
    {
        HttpClient client = CreateClient();

        using MultipartFormDataContent content = [];
        using Stream fileStream = file.OpenReadStream();
        using StreamContent streamContent = new(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.FileName);

        if (!string.IsNullOrEmpty(description))
        {
            content.Add(new StringContent(description), "description");
        }

        HttpResponseMessage response = await client.PostAsync(
            $"{ApiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents",
            content,
            ct);

        return await ProxyResponse(response, ct);
    }

    /// <summary>
    /// Proxy document download from the API server
    /// </summary>
    [HttpGet("{documentDisplayId:long}/download")]
    public async Task<IActionResult> DownloadDocument(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken ct)
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.GetAsync(
            $"{ApiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents/{documentDisplayId}/download",
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        string contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        string fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "document";
        Stream stream = await response.Content.ReadAsStreamAsync(ct);

        return File(stream, contentType, fileName);
    }

    /// <summary>
    /// Proxy profile image upload to the API server
    /// </summary>
    [HttpPost("profile-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfileImage(
        long personDisplayId,
        [FromForm] IFormFile file,
        CancellationToken ct)
    {
        HttpClient client = CreateClient();

        using MultipartFormDataContent content = [];
        using Stream fileStream = file.OpenReadStream();
        using StreamContent streamContent = new(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.FileName);

        HttpResponseMessage response = await client.PostAsync(
            $"{ApiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents/profile-image",
            content,
            ct);

        return await ProxyResponse(response, ct);
    }

    /// <summary>
    /// Proxy profile image retrieval from the API server
    /// </summary>
    [HttpGet("profile-image")]
    public async Task<IActionResult> GetProfileImage(
        long personDisplayId,
        CancellationToken ct)
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.GetAsync(
            $"{ApiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents/profile-image",
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        string contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
        Stream stream = await response.Content.ReadAsStreamAsync(ct);

        return File(stream, contentType);
    }

    /// <summary>
    /// Proxy profile image deletion to the API server
    /// </summary>
    [HttpDelete("profile-image")]
    public async Task<IActionResult> DeleteProfileImage(
        long personDisplayId,
        CancellationToken ct)
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.DeleteAsync(
            $"{ApiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents/profile-image",
            ct);

        return StatusCode((int)response.StatusCode);
    }

    /// <summary>
    /// Proxy document deletion to the API server
    /// </summary>
    [HttpDelete("{documentDisplayId:long}")]
    public async Task<IActionResult> DeleteDocument(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken ct)
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.DeleteAsync(
            $"{ApiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents/{documentDisplayId}",
            ct);

        return StatusCode((int)response.StatusCode);
    }

    private HttpClient CreateClient()
    {
        HttpClient client = _httpClientFactory.CreateClient("EmsApiClient");

        // Forward the Authorization header from the incoming request
        if (Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
        {
            _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authHeader.ToString());
        }

        return client;
    }

    private async Task<IActionResult> ProxyResponse(HttpResponseMessage response, CancellationToken ct)
    {
        string content = await response.Content.ReadAsStringAsync(ct);

        return response.IsSuccessStatusCode
            ? Content(content, response.Content.Headers.ContentType?.ToString() ?? "application/json")
            : StatusCode((int)response.StatusCode, content);
    }
}
