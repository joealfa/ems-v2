using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Authentication;
using EmployeeManagementSystem.Gateway.Caching;
using EmployeeManagementSystem.Gateway.DataLoaders;
using HotChocolate.Types.Descriptors;
using StackExchange.Redis;

namespace EmployeeManagementSystem.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method for service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds all gateway services to the service collection.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddGatewayServices(IConfiguration configuration)
        {
            // Configure Redis
            _ = services.AddRedisServices(configuration);

            // Configure EMS API Client
            _ = services.AddEmsApiClient(configuration);

            // Configure HotChocolate GraphQL
            _ = services.AddGraphQLServices();

            return services;
        }

        /// <summary>
        /// Adds Redis caching services to the service collection.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        private IServiceCollection AddRedisServices(IConfiguration configuration)
        {
            // TEMPORARY: Disable Redis caching for debugging
            // Use NoOpCacheService which bypasses all caching
            _ = services.AddScoped<IRedisCacheService, NoOpCacheService>();

            // Original Redis implementation (commented out):
            /*
            string redisConnectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";
            string instanceName = configuration["Redis:InstanceName"] ?? "EmsGateway:";

            // Register Redis connection multiplexer
            _ = services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnectionString));

            // Register distributed cache
            _ = services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = instanceName;
            });

            // Register custom cache service
            _ = services.AddScoped<IRedisCacheService, RedisCacheService>();
            */

            return services;
        }

        /// <summary>
        /// Adds the EMS API client to the service collection.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        private IServiceCollection AddEmsApiClient(IConfiguration configuration)
        {
            string baseUrl = configuration["ApiClient:BaseUrl"] ?? "https://localhost:5001";

            // Get the HTTP context accessor to forward auth token
            _ = services.AddHttpContextAccessor();

            // Register the JWT forwarding handler
            _ = services.AddTransient<JwtForwardingHandler>();

            _ = services.AddHttpClient("EmsApiClient")
                .AddHttpMessageHandler<JwtForwardingHandler>();

            // Register the EMS API client with token forwarding from GraphQL context
            _ = services.AddScoped(sp =>
            {
                IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                HttpClient httpClient = httpClientFactory.CreateClient("EmsApiClient");

                return new EmsApiClient(httpClient) { BaseUrl = baseUrl };
            });

            return services;
        }

        /// <summary>
        /// Adds GraphQL services to the service collection.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        private IServiceCollection AddGraphQLServices()
        {
            // Register custom naming convention for PascalCase enum values
            _ = services.AddSingleton<INamingConventions, Types.PascalCaseNamingConventions>();

            _ = services
                .AddGraphQLServer()
                .AddQueryType<Types.Query>()
                .AddMutationType<Types.Mutation>()
                .AddType<UploadType>()
                // Register custom Long scalar that accepts both string and numeric input
                .AddType<Types.LongType>()
                .AddTypeExtension<Types.PersonResponseDtoExtensions>()
                .AddTypeExtension<Types.PersonListDtoExtensions>()
                .AddTypeExtension<Types.PagedResultOfPersonListDtoExtensions>()
                .AddTypeExtension<Types.PagedResultOfDocumentListDtoExtensions>()
                .AddTypeExtension<Types.EmploymentResponseDtoExtensions>()
                .AddTypeExtension<Types.EmploymentListDtoExtensions>()
                .AddTypeExtension<Types.EmploymentPersonDtoExtensions>()
                .AddTypeExtension<Types.EmploymentPositionDtoExtensions>()
                .AddTypeExtension<Types.EmploymentSalaryGradeDtoExtensions>()
                .AddTypeExtension<Types.EmploymentItemDtoExtensions>()
                .AddTypeExtension<Types.EmploymentSchoolResponseDtoExtensions>()
                .AddTypeExtension<Types.PagedResultOfEmploymentListDtoExtensions>()
                .AddTypeExtension<Types.SchoolResponseDtoExtensions>()
                .AddTypeExtension<Types.SchoolListDtoExtensions>()
                .AddTypeExtension<Types.PagedResultOfSchoolListDtoExtensions>()
                .AddTypeExtension<Types.PositionResponseDtoExtensions>()
                .AddTypeExtension<Types.PagedResultOfPositionResponseDtoExtensions>()
                .AddTypeExtension<Types.SalaryGradeResponseDtoExtensions>()
                .AddTypeExtension<Types.PagedResultOfSalaryGradeResponseDtoExtensions>()
                .AddTypeExtension<Types.ItemResponseDtoExtensions>()
                .AddTypeExtension<Types.PagedResultOfItemResponseDtoExtensions>()
                .AddTypeExtension<Types.AddressResponseDtoExtensions>()
                .AddTypeExtension<Types.ContactResponseDtoExtensions>()
                .AddTypeExtension<Types.DocumentListDtoExtensions>()
                .AddTypeExtension<Types.DocumentResponseDtoExtensions>()
                .AddTypeExtension<Types.DashboardStatsDtoExtensions>()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
                .AddFiltering()
                .AddSorting()
                .AddProjections();

            // Register DataLoaders
            _ = services.AddScoped<PersonDataLoader>();
            _ = services.AddScoped<SchoolDataLoader>();
            _ = services.AddScoped<EmploymentDataLoader>();
            _ = services.AddScoped<PositionDataLoader>();
            _ = services.AddScoped<SalaryGradeDataLoader>();
            _ = services.AddScoped<ItemDataLoader>();

            return services;
        }
    }
}
