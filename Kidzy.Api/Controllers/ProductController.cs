using Kidzy.Application.DTOs.Product;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(
            IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products =
                await _productService.GetAllAsync();

            return Ok(products);
        }

        // GET: api/Product/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product =
                await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create(
            ProductCreateDto dto)
        {
            var product =
                await _productService.CreateAsync(dto);

            if (product == null)
            {
                return BadRequest(new
                {
                    message = "Unable to create product"
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }

        // DELETE: api/Product/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted =
                await _productService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(new
            {
                message = "Product deleted successfully"
            });
        }
    }
}