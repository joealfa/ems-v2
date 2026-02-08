using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.Infrastructure.Data;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances.
/// Used by EF Core tools (migrations, scaffolding) to avoid running full application startup.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // When EF tools run, they typically execute from the startup project directory
        // If not, we need to navigate to find the API project's configuration
        string basePath = Directory.GetCurrentDirectory();

        // Check if appsettings.json exists in current directory
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            // Try to find the API project directory
            string? apiProjectPath = FindApiProjectPath(basePath);
            basePath = apiProjectPath ?? throw new InvalidOperationException(
                    $"Could not locate API project configuration files from: {basePath}. " +
                    "Please run EF commands from the API project directory or specify --startup-project.");
        }

        // Build configuration to read connection string
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Try to add user secrets - use the API project's UserSecretsId
        const string userSecretsId = "6d3bfc92-af45-453d-aa90-b6da41f650cf";
        string userSecretsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft", "UserSecrets", userSecretsId, "secrets.json");

        if (File.Exists(userSecretsPath))
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile(userSecretsPath, optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        // Get connection string
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. " +
                "Please configure it in appsettings.json, user secrets, or environment variables.");
        }

        // Build DbContext options
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
        _ = optionsBuilder.UseSqlServer(
            connectionString,
            b => b.MigrationsAssembly("EmployeeManagementSystem.Infrastructure"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string? FindApiProjectPath(string currentPath)
    {
        // Look for EmployeeManagementSystem.Api directory
        string? directory = currentPath;

        for (int i = 0; i < 3; i++) // Search up to 3 levels
        {
            if (directory == null)
            {
                break;
            }

            string apiPath = Path.Combine(directory, "EmployeeManagementSystem.Api");
            if (Directory.Exists(apiPath) && File.Exists(Path.Combine(apiPath, "appsettings.json")))
            {
                return apiPath;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        return null;
    }
}
