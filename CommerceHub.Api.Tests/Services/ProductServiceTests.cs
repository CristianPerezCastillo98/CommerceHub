using CommerceHub.Library.Models.Request;
using CommerceHub.Library.Services;
using CommerceHub.Persistence.Models;
using CommerceHub.Persistence.Repositories.Interfaces;
using Moq;

namespace CommerceHub.Api.Tests.Services;

public class ProductServiceTests
{
    private const long ProductId = 1;

    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private ProductService _productService;

    [SetUp]
    public void SetUp()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productService = new ProductService(_productRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    #region GetByIdAsync

    [Test]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var product = CreateProduct();

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(product.Id, _cancellationToken)).ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(product.Id, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(product.Id));
            Assert.That(result.Name, Is.EqualTo(product.Name));
            Assert.That(result.Description, Is.EqualTo(product.Description));
            Assert.That(result.Price, Is.EqualTo(product.Price));
            Assert.That(result.Stock, Is.EqualTo(product.Stock));
            Assert.That(result.CreatedAt, Is.EqualTo(product.CreatedAt));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(ProductId, _cancellationToken)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetByIdAsync(ProductId, _cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion GetByIdAsync

    #region CreateAsync

    [Test]
    public async Task CreateAsync_WithValidRequest_ShouldCreateProduct()
    {
        // Arrange
        var request = CreateProductRequest();

        // Act
        var result = await _productService.CreateAsync(request, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Description, Is.EqualTo(request.Description));
            Assert.That(result.Price, Is.EqualTo(request.Price));
            Assert.That(result.Stock, Is.EqualTo(request.Stock));
            Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));
        }

        _productRepositoryMock.Verify(
            repo => repo.CreateAsync(
                It.Is<Product>(product =>
                    product.Name == request.Name &&
                    product.Description == request.Description &&
                    product.Price == request.Price &&
                    product.Stock == request.Stock &&
                    product.CreatedAt != default),
                _cancellationToken),
            Times.Once);

        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    #endregion CreateAsync

    #region UpdateAsync

    [Test]
    public async Task UpdateAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var request = CreateProductRequest();

        _productRepositoryMock.Setup(repo => repo.GetTrackedByIdAsync(ProductId, _cancellationToken)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.UpdateAsync(ProductId, request, _cancellationToken);

        // Assert
        Assert.That(result, Is.Null);

        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(_cancellationToken), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WhenProductExists_ShouldUpdateProduct()
    {
        // Arrange
        var request = CreateProductRequest();
        var product = CreateProduct();
        var createdAt = DateTime.UtcNow.AddDays(-10);

        product.CreatedAt = createdAt;

        _productRepositoryMock.Setup(repo => repo.GetTrackedByIdAsync(product.Id, _cancellationToken)).ReturnsAsync(product);

        // Act
        var result = await _productService.UpdateAsync(product.Id, request, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(product.Id));
            Assert.That(result.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Description, Is.EqualTo(request.Description));
            Assert.That(result.Price, Is.EqualTo(request.Price));
            Assert.That(result.Stock, Is.EqualTo(request.Stock));

            Assert.That(product.Name, Is.EqualTo(request.Name));
            Assert.That(product.Description, Is.EqualTo(request.Description));
            Assert.That(product.Price, Is.EqualTo(request.Price));
            Assert.That(product.Stock, Is.EqualTo(request.Stock));
            Assert.That(product.CreatedAt, Is.EqualTo(createdAt));
        }

        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    #endregion UpdateAsync

    #region DeleteAsync

    [Test]
    public async Task DeleteAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(ProductId, _cancellationToken)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteAsync(ProductId, _cancellationToken);

        // Assert
        Assert.That(result, Is.False);

        _productRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(_cancellationToken), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_WhenProductExists_ShouldDeleteProduct()
    {
        // Arrange
        var product = CreateProduct();

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(ProductId, _cancellationToken)).ReturnsAsync(product);

        // Act
        var result = await _productService.DeleteAsync(ProductId, _cancellationToken);

        // Assert
        Assert.That(result, Is.True);

        _productRepositoryMock.Verify(repo => repo.Delete(product), Times.Once);
        _unitOfWorkMock.Verify(unit => unit.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    #endregion DeleteAsync

    #region Test Data

    private static Product CreateProduct()
    {
        return new Product
        {
            Id = ProductId,
            Name = "Keyboard",
            Description = "Mechanical keyboard",
            Price = 99.99m,
            Stock = 10,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static ProductRequest CreateProductRequest()
    {
        return new ProductRequest
        {
            Name = "Keyboard Pro",
            Description = "Mechanical keyboard pro",
            Price = 129.99m,
            Stock = 5
        };
    }

    #endregion Test Data
}