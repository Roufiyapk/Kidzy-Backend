using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly KidzyDbContext _context;

        public ProductRepository(KidzyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // CREATE PRODUCT
        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return product;
        }

        // DELETE PRODUCT
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}