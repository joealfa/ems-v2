
using Microsoft.Extensions.DependencyInjection;
using EmployeeManagementSystem.ApiClient.Authentication;

namespace EmployeeManagementSystem.ApiClient.Extensions;

/// <summary>
/// Extension methods for registering the EMS API client with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the EMS API client to the service collection with a dynamic token provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the EMS API.</param>
    /// <param name="tokenProvider">
    /// A factory function that takes the service provider and returns a token provider function.
    /// The token provider function receives a cancellation token and returns the JWT token.
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmsApiClient(
        this IServiceCollection services,
        string baseUrl,
        Func<IServiceProvider, Func<CancellationToken, Task<string?>>> tokenProvider)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentNullException.ThrowIfNull(tokenProvider);

        services.AddHttpClient("EmsApiClient")
            .ConfigurePrimaryHttpMessageHandler(sp => new JwtAuthenticationHandler(tokenProvider(sp)));

        services.AddScoped(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("EmsApiClient");

            return new Generated.EmsApiClient(httpClient) { BaseUrl = baseUrl };
        });

        return services;
    }

    /// <summary>
    /// Adds the EMS API client to the service collection with a static token.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the EMS API.</param>
    /// <param name="token">The static JWT token to use for authentication.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmsApiClient(
        this IServiceCollection services,
        string baseUrl,
        string token)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        services.AddHttpClient("EmsApiClient")
            .ConfigurePrimaryHttpMessageHandler(() => new JwtAuthenticationHandler(token));

        services.AddScoped(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("EmsApiClient");

            return new Generated.EmsApiClient(httpClient) { BaseUrl = baseUrl };
        });

        return services;
    }

    /// <summary>
    /// Adds the EMS API client to the service collection without authentication.
    /// Use this for public endpoints or when authentication is handled externally.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the EMS API.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmsApiClientWithoutAuth(
        this IServiceCollection services,
        string baseUrl)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

        services.AddHttpClient("EmsApiClient");

        services.AddScoped(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("EmsApiClient");

            return new Generated.EmsApiClient(httpClient) { BaseUrl = baseUrl };
        });

        return services;
    }
}
