using CommerceHub.Library.Services;
using CommerceHub.Library.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceHub.Library.Extensions;

public static class LibraryExtension
{
    public static IServiceCollection AddLibrary(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}