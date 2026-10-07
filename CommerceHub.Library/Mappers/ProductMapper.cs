using CommerceHub.Library.Models.Request;
using CommerceHub.Library.Models.Response;
using CommerceHub.Persistence.Models;

namespace CommerceHub.Library.Mappers;

public static class ProductMapper
{
    public static Product ToProduct(this ProductRequest request)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };
    }

    public static void MapTo(this ProductRequest request, Product product)
    {
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
    }
}