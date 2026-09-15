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

    public async Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
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
}