using CommerceHub.Library.Mappers;
using CommerceHub.Library.Models.Request;
using CommerceHub.Library.Models.Response;
using CommerceHub.Library.Services.Interfaces;
using CommerceHub.Persistence.Repositories.Interfaces;

namespace CommerceHub.Library.Services;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<List<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);

        return [.. products.Select(product => product.ToResponse())];
    }

    public async Task<ProductResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        return product?.ToResponse();
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        var product = request.ToProduct();

        await productRepository.CreateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToResponse();
    }

    public async Task<ProductResponse?> UpdateAsync(long id, ProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetTrackedByIdAsync(id, cancellationToken);

        if (product is null)
            return null;

        request.MapTo(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product.ToResponse();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return false;

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}