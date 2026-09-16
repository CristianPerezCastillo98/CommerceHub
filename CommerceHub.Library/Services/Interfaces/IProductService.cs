using CommerceHub.Library.Models.Request;
using CommerceHub.Persistence.Models;

namespace CommerceHub.Library.Services.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Product> CreateAsync(ProductRequest request, CancellationToken cancellationToken);

    Task<Product?> UpdateAsync(long id, ProductRequest newProduct, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}