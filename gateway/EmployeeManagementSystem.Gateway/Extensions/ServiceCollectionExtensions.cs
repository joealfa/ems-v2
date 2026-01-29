using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Authentication;
using EmployeeManagementSystem.Gateway.Caching;
using EmployeeManagementSystem.Gateway.DataLoaders;
using StackExchange.Redis;

namespace EmployeeManagementSystem.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Redis
        _ = services.AddRedisServices(configuration);

        // Configure EMS API Client
        _ = services.AddEmsApiClient(configuration);

        // Configure HotChocolate GraphQL
        _ = services.AddGraphQLServices();

        return services;
    }

    private static IServiceCollection AddRedisServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
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

        return services;
    }

    private static IServiceCollection AddEmsApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
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

    private static IServiceCollection AddGraphQLServices(this IServiceCollection services)
    {
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
