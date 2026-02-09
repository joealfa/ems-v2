using EmployeeManagementSystem.ApiClient.Extensions;
using EmployeeManagementSystem.Gateway.Caching;
using EmployeeManagementSystem.Gateway.DataLoaders;
using EmployeeManagementSystem.Gateway.Messaging;
using EmployeeManagementSystem.Gateway.Services;
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

            // Configure RabbitMQ event consumer
            _ = services.AddRabbitMQServices(configuration);

            // Register ActivityEventBuffer for subscription history
            _ = services.AddSingleton<ActivityEventBuffer>();

            // Register HttpContextAccessor for token forwarding
            _ = services.AddHttpContextAccessor();

            // Use the ApiClient's AddEmsApiClient with a token provider that extracts the token from HTTP context
            _ = services.AddEmsApiClient(configuration["ApiClient:BaseUrl"] ?? "https://localhost:5001", sp =>
            {
                IHttpContextAccessor httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                return _ => Task.FromResult(
                    httpContextAccessor.HttpContext?.Request.Headers.Authorization
                        .FirstOrDefault()?.Replace("Bearer ", ""));
            });

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

            // Register custom cache service with hash-based key generation
            _ = services.AddScoped<IRedisCacheService, RedisCacheService>();

            return services;
        }

        /// <summary>
        /// Adds RabbitMQ event consumer services to the service collection.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        private IServiceCollection AddRabbitMQServices(IConfiguration configuration)
        {
            // Configure RabbitMQ settings
            _ = services.Configure<RabbitMQSettings>(configuration.GetSection(RabbitMQSettings.SectionName));

            // Register RabbitMQ event consumer as singleton (single instance for the app)
            _ = services.AddSingleton<RabbitMQEventConsumer>();

            // Register background service to run the consumer
            _ = services.AddHostedService<RabbitMQBackgroundService>();

            return services;
        }

        /// <summary>
        /// Adds GraphQL services to the service collection.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        private IServiceCollection AddGraphQLServices()
        {
            // Register custom naming convention for PascalCase enum values
            _ = services.AddSingleton<INamingConventions, Types.Configuration.PascalCaseNamingConventions>();

            _ = services
                .AddGraphQLServer()
                .AddErrorFilter<Errors.ApiExceptionErrorFilter>()
                .AddQueryType<Types.Query>()
                .AddMutationType<Types.Mutation>()
                .AddSubscriptionType<Types.Subscription>()
                .AddInMemorySubscriptions()
                .AddType<UploadType>()
                // Register custom Long scalar that accepts both string and numeric input
                .AddType<Types.Configuration.LongType>()
                .AddTypeExtension<Types.Extensions.PersonResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PersonListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PagedResultOfPersonListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PagedResultOfDocumentListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.EmploymentResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.EmploymentListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.EmploymentPersonDtoExtensions>()
                .AddTypeExtension<Types.Extensions.EmploymentPositionDtoExtensions>()
                .AddTypeExtension<Types.Extensions.EmploymentSalaryGradeDtoExtensions>()
                .AddTypeExtension<Types.Extensions.EmploymentItemDtoExtensions>()
                .AddTypeExtension<Types.Extensions.EmploymentSchoolResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PagedResultOfEmploymentListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.SchoolResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.SchoolListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PagedResultOfSchoolListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PositionResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PagedResultOfPositionResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.SalaryGradeResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PagedResultOfSalaryGradeResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.ItemResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.PagedResultOfItemResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.AddressResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.ContactResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.DocumentListDtoExtensions>()
                .AddTypeExtension<Types.Extensions.DocumentResponseDtoExtensions>()
                .AddTypeExtension<Types.Extensions.BirthdayCelebrantDtoExtensions>()
                .AddTypeExtension<Types.Extensions.RecentActivityDtoExtensions>()
                .AddTypeExtension<Types.Extensions.DashboardStatsDtoExtensions>()
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
