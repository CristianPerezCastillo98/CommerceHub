using CommerceHub.Persistence.Context;
using CommerceHub.Persistence.Models;
using CommerceHub.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.Persistence.Repositories;

public class ProductRepository(CommerceHubDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Products.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Product?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken)
    {
        return context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task CreateAsync(Product product, CancellationToken cancellationToken)
    {
        await context.Products.AddAsync(product, cancellationToken);
    }

    public void Delete(Product product)
    {
        context.Products.Remove(product);
    }
}