using CommerceHub.Library.Models.Request;
using CommerceHub.Library.Services.Interfaces;
using CommerceHub.Persistence.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommerceHub.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts(CancellationToken cancellationToken)
        {
            return Ok(await productService.GetAllAsync(cancellationToken));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id, CancellationToken cancellationToken)
        {
            var product = await productService.GetByIdAsync(id, cancellationToken);

            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductRequest product, CancellationToken cancellationToken)
        {
            var newProduct = await productService.CreateAsync(product, cancellationToken);

            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
        }
    }
}