
using EmployeeManagementSystem.ApiClient.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementSystem.ApiClient.Extensions;

/// <summary>
/// Extension methods for registering the EMS API client with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method for service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the EMS API client to the service collection with a dynamic token provider.
        /// </summary>
        /// <param name="baseUrl">The base URL of the EMS API.</param>
        /// <param name="tokenProvider">
        /// A factory function that takes the service provider and returns a token provider function.
        /// The token provider function receives a cancellation token and returns the JWT token.
        /// </param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddEmsApiClient(
            string baseUrl,
            Func<IServiceProvider, Func<CancellationToken, Task<string?>>> tokenProvider)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
            ArgumentNullException.ThrowIfNull(tokenProvider);

            _ = services.AddHttpClient("EmsApiClient")
                .ConfigurePrimaryHttpMessageHandler(sp => new JwtAuthenticationHandler(tokenProvider(sp)));

            _ = services.AddScoped(sp =>
            {
                IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                HttpClient httpClient = httpClientFactory.CreateClient("EmsApiClient");

                return new Generated.EmsApiClient(httpClient) { BaseUrl = baseUrl };
            });

            return services;
        }

        /// <summary>
        /// Adds the EMS API client to the service collection with a static token.
        /// </summary>
        /// <param name="baseUrl">The base URL of the EMS API.</param>
        /// <param name="token">The static JWT token to use for authentication.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddEmsApiClient(
            string baseUrl,
            string token)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
            ArgumentException.ThrowIfNullOrWhiteSpace(token);

            _ = services.AddHttpClient("EmsApiClient")
                .ConfigurePrimaryHttpMessageHandler(() => new JwtAuthenticationHandler(token));

            _ = services.AddScoped(sp =>
            {
                IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                HttpClient httpClient = httpClientFactory.CreateClient("EmsApiClient");

                return new Generated.EmsApiClient(httpClient) { BaseUrl = baseUrl };
            });

            return services;
        }

        /// <summary>
        /// Adds the EMS API client to the service collection without authentication.
        /// Use this for public endpoints or when authentication is handled externally.
        /// </summary>
        /// <param name="baseUrl">The base URL of the EMS API.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddEmsApiClientWithoutAuth(string baseUrl)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

            _ = services.AddHttpClient("EmsApiClient");

            _ = services.AddScoped(sp =>
            {
                IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                HttpClient httpClient = httpClientFactory.CreateClient("EmsApiClient");

                return new Generated.EmsApiClient(httpClient) { BaseUrl = baseUrl };
            });

            return services;
        }
    }
}
