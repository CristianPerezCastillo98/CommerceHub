using CommerceHub.Persistence.Models;

namespace CommerceHub.Persistence.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Product?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);

    Task CreateAsync(Product product, CancellationToken cancellationToken);

    void Delete(Product product);
}