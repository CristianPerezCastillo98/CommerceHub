using CommerceHub.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommerceHub.IntegrationTests.Infrastructure;

public class CommerceHubWebApplicationFactory : WebApplicationFactory<Program>
{
    private string? _testConnectionString;

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<CommerceHubDbContext>();

        await context.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, configuration) =>
        {
            configuration.AddUserSecrets<
                CommerceHubWebApplicationFactory>();

            var settings = configuration.Build();

            _testConnectionString = settings.GetConnectionString(
                "CommerceHubTestDatabase");

            if (string.IsNullOrWhiteSpace(_testConnectionString))
            {
                throw new InvalidOperationException(
                    "CommerceHubTestDatabase is not configured.");
            }

            var developmentConnection = settings.GetConnectionString(
                "CommerceHubDatabase");

            if (!string.IsNullOrWhiteSpace(developmentConnection))
            {
                var testDatabase =
                    new Npgsql.NpgsqlConnectionStringBuilder(
                        _testConnectionString);

                var developmentDatabase =
                    new Npgsql.NpgsqlConnectionStringBuilder(
                        developmentConnection);

                if (string.Equals(
                        testDatabase.Host,
                        developmentDatabase.Host,
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    string.Equals(
                        testDatabase.Database,
                        developmentDatabase.Database,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Integration tests cannot use the development database.");
                }
            }
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<CommerceHubDbContext>>();

            services.AddDbContext<CommerceHubDbContext>(
                options => options.UseNpgsql(_testConnectionString!));
        });
    }
}