using CommerceHub.IntegrationTests.Infrastructure;
using CommerceHub.Library.Models.Request;
using CommerceHub.Library.Models.Response;
using CommerceHub.Persistence.Models;
using System.Net;
using System.Net.Http.Json;

namespace CommerceHub.IntegrationTests.Products;

public class ProductEndpointTests
{
    private CommerceHubWebApplicationFactory _factory = null!;
    private DatabaseHelper _database = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _factory = new CommerceHubWebApplicationFactory();

        await _factory.InitializeDatabaseAsync();

        _database = new DatabaseHelper(_factory);
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    #region GetProducts

    [Test]
    public async Task GetProducts_ShouldReturnOk()
    {
        // Act
        using var response = await _client.GetAsync("/api/v1/products");
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(products, Is.Not.Null);
        }
    }

    #endregion GetProducts

    #region GetProduct

    [Test]
    public async Task GetProduct_WhenProductExists_ShouldReturnOk()
    {
        // Arrange
        var product = CreateProduct();
        var createdProduct = await _database.CreateProductAsync(product);

        try
        {
            // Act
            using var response = await _client.GetAsync($"/api/v1/products/{createdProduct.Id}");
            var retrievedProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(retrievedProduct, Is.Not.Null);

                Assert.That(retrievedProduct?.Id, Is.EqualTo(createdProduct.Id));
                Assert.That(retrievedProduct?.Name, Is.EqualTo(createdProduct.Name));
                Assert.That(retrievedProduct?.Description, Is.EqualTo(createdProduct.Description));
                Assert.That(retrievedProduct?.Price, Is.EqualTo(createdProduct.Price));
                Assert.That(retrievedProduct?.Stock, Is.EqualTo(createdProduct.Stock));
            }
        }
        finally
        {
            await _database.DeleteProductAsync(createdProduct.Id);
        }
    }

    [Test]
    public async Task GetProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var product = await _database.CreateProductAsync(CreateProduct());
        var productId = product.Id;

        await _database.DeleteProductAsync(productId);

        // Act
        using var response = await _client.GetAsync($"/api/v1/products/{productId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion GetProduct

    #region CreateProduct

    [Test]
    public async Task CreateProduct_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = CreateProductRequest();
        long? createdProductId = null;

        try
        {
            // Act
            using var response = await _client.PostAsJsonAsync("/api/v1/products", request);
            var product = await response.Content.ReadFromJsonAsync<ProductResponse>();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(product, Is.Not.Null);
                Assert.That(product?.Id, Is.GreaterThan(0));
                Assert.That(product?.Name, Is.EqualTo(request.Name));
                Assert.That(product?.Description, Is.EqualTo(request.Description));
                Assert.That(product?.Price, Is.EqualTo(request.Price));
                Assert.That(product?.Stock, Is.EqualTo(request.Stock));
            }

            if (product is not null)
            {
                createdProductId = product.Id;

                using var getResponse = await _client.GetAsync($"/api/v1/products/{product.Id}");

                Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
        }
        finally
        {
            if (createdProductId.HasValue)
            {
                await _database.DeleteProductAsync(createdProductId.Value);
            }
        }
    }

    #endregion CreateProduct

    #region UpdateProduct

    [Test]
    public async Task UpdateProduct_WhenProductExists_ShouldReturnOkAndPersistChanges()
    {
        // Arrange
        var product = await _database.CreateProductAsync(CreateProduct());
        var request = CreateProductRequest();

        try
        {
            // Act
            using var response = await _client.PutAsJsonAsync($"/api/v1/products/{product.Id}", request);
            var updatedProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();
            using var getResponse = await _client.GetAsync($"/api/v1/products/{product.Id}");
            var persistedProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(updatedProduct, Is.Not.Null);
                Assert.That(updatedProduct?.Id, Is.EqualTo(product.Id));
                Assert.That(updatedProduct?.Name, Is.EqualTo(request.Name));
                Assert.That(updatedProduct?.Description, Is.EqualTo(request.Description));
                Assert.That(updatedProduct?.Price, Is.EqualTo(request.Price));
                Assert.That(updatedProduct?.Stock, Is.EqualTo(request.Stock));

                Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(persistedProduct, Is.Not.Null);
                Assert.That(persistedProduct?.Id, Is.EqualTo(product.Id));
                Assert.That(persistedProduct?.Name, Is.EqualTo(request.Name));
                Assert.That(persistedProduct?.Description, Is.EqualTo(request.Description));
                Assert.That(persistedProduct?.Price, Is.EqualTo(request.Price));
                Assert.That(persistedProduct?.Stock, Is.EqualTo(request.Stock));
            }
        }
        finally
        {
            await _database.DeleteProductAsync(product.Id);
        }
    }

    [Test]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var product = await _database.CreateProductAsync(CreateProduct());
        var productId = product.Id;

        await _database.DeleteProductAsync(productId);

        var request = CreateProductRequest();

        // Act
        using var response = await _client.PutAsJsonAsync($"/api/v1/products/{productId}", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion UpdateProduct

    #region DeleteProduct

    [Test]
    public async Task DeleteProduct_WhenProductExists_ShouldReturnNoContentAndDeleteProduct()
    {
        // Arrange
        var product = await _database.CreateProductAsync(CreateProduct());

        // Act
        using var response = await _client.DeleteAsync($"/api/v1/products/{product.Id}");
        using var getResponse = await _client.GetAsync($"/api/v1/products/{product.Id}");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }

    [Test]
    public async Task DeleteProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var product = await _database.CreateProductAsync(CreateProduct());
        var productId = product.Id;

        await _database.DeleteProductAsync(productId);

        // Act
        using var response = await _client.DeleteAsync($"/api/v1/products/{productId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion DeleteProduct

    #region TestData

    private static Product CreateProduct()
    {
        return new Product
        {
            Name = "Integration Test Keyboard",
            Description = "Integration test product",
            Price = 99.99m,
            Stock = 10,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static ProductRequest CreateProductRequest()
    {
        return new ProductRequest
        {
            Name = "Integration Test Keyboard",
            Description = "Created from integration testing",
            Price = 89.99m,
            Stock = 10
        };
    }

    #endregion TestData
}