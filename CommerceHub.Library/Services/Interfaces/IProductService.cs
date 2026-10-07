using CommerceHub.Library.Models.Request;
using CommerceHub.Library.Models.Response;

namespace CommerceHub.Library.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(long id, ProductRequest newProduct, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}