using CommerceHub.Library.Services;
using CommerceHub.Library.Services.Interfaces;
using CommerceHub.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceHub.Library.Extensions;

public static class LibraryExtension
{
    public static IServiceCollection AddLibrary(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}