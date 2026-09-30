using CommerceHub.Persistence.Context;
using CommerceHub.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceHub.IntegrationTests.Infrastructure;

public class DatabaseHelper(
    CommerceHubWebApplicationFactory factory)
{
    public async Task<Product> CreateProductAsync(Product product)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CommerceHubDbContext>();

        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        return product;
    }

    public async Task DeleteProductAsync(long id)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CommerceHubDbContext>();
        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return;

        context.Products.Remove(product);

        await context.SaveChangesAsync();
    }
}