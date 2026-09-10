using Kidzy.Application.DTOs.Product;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(
            IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET ALL PRODUCTS
        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products =
                await _productRepository.GetAllAsync();

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,

                Name = p.Name,

                Price = p.Price,

                CategoryId = p.CategoryId,

                CategoryName = p.Category.Name,

                Age = p.Age,

                Image = p.Image,

                Description = p.Description,

                BestSeller = p.BestSeller,

                NewArrival = p.NewArrival,

                Sizes = p.ProductSizes
                    .Select(ps => new ProductSizeDto
                    {
                        Size = ps.Size,
                        Stock = ps.Stock
                    })
                    .ToList()

            }).ToList();
        }

        // GET PRODUCT BY ID
        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return MapToDto(product);
        }

        // CREATE PRODUCT
        public async Task<ProductResponseDto?> CreateAsync(
            ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,

                Price = dto.Price,

                CategoryId = dto.CategoryId,

                Age = dto.Age,

                Image = dto.Image,

                Description = dto.Description,

                BestSeller = dto.BestSeller,

                NewArrival = dto.NewArrival,

                ProductSizes = dto.Sizes
                    .Select(size => new ProductSize
                    {
                        Size = size.Size,
                        Stock = size.Stock
                    })
                    .ToList()
            };

            var createdProduct =
                await _productRepository.CreateAsync(product);

            // Get created product with Category and Sizes
            var result =
                await _productRepository.GetByIdAsync(
                    createdProduct.Id);

            if (result == null)
            {
                return null;
            }

            return MapToDto(result);
        }

        // DELETE PRODUCT
        public async Task<bool> DeleteAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        // MAPPING
        private static ProductResponseDto MapToDto(
            Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,

                Name = product.Name,

                Price = product.Price,

                CategoryId = product.CategoryId,

                CategoryName = product.Category.Name,

                Age = product.Age,

                Image = product.Image,

                Description = product.Description,

                BestSeller = product.BestSeller,

                NewArrival = product.NewArrival,

                Sizes = product.ProductSizes
                    .Select(ps => new ProductSizeDto
                    {
                        Size = ps.Size,
                        Stock = ps.Stock
                    })
                    .ToList()
            };
        }
    }
}