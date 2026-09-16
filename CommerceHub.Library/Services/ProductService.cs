using CommerceHub.Library.Models.Request;
using CommerceHub.Library.Services.Interfaces;
using CommerceHub.Persistence.Models;
using CommerceHub.Persistence.Repositories.Interfaces;

namespace CommerceHub.Library.Services;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await productRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await productRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Product> CreateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        var newProduct = new Product
        {
            Description = request.Description,
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow,
        };

        await productRepository.CreateAsync(newProduct, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newProduct;
    }

    public async Task<Product?> UpdateAsync(long id, ProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetTrackedByIdAsync(id, cancellationToken);

        if (product is null)
            return null;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product;
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