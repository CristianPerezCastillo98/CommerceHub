using CommerceHub.Persistence.Context;
using CommerceHub.Persistence.Repositories;
using CommerceHub.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceHub.Persistence.Extensions;

public static class PersistenceExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CommerceHubDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("CommerceHubDatabase")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CommerceHubDbContext>());

        return services;
    }
}