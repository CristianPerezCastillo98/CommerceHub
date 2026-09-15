using CommerceHub.Library.Models.Request;
using CommerceHub.Persistence.Models;

namespace CommerceHub.Library.Services.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
}