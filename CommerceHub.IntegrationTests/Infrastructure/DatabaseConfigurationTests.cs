using CommerceHub.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace CommerceHub.IntegrationTests.Infrastructure;

public class DatabaseConfigurationTests
{
    [Test]
    public void DbContext_ShouldUseTestingDatabase()
    {
        // Arrange
        using var factory = new CommerceHubWebApplicationFactory();
        using var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<CommerceHubDbContext>();

        // Act
        var connection = context.Database.GetDbConnection();
        var connectionSettings = new NpgsqlConnectionStringBuilder(connection.ConnectionString);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(connectionSettings.Database, Is.EqualTo("neondb"));
            Assert.That(connectionSettings.Host, Is.EqualTo("ep-plain-resonance-za19jmi2.c-2.eu-west-2.aws.neon.tech"));
        });
    }

    [Test]
    public async Task InitializeDatabase_ShouldApplyMigrations()
    {
        // Arrange
        using var factory = new CommerceHubWebApplicationFactory();

        // Act
        await factory.InitializeDatabaseAsync();

        // Assert
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CommerceHubDbContext>();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        Assert.That(pendingMigrations, Is.Empty);
    }
}