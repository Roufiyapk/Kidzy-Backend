using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // GET ALL PRODUCTS

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .Include(p => p.Variants)
            .AsNoTracking()
            .ToListAsync();
    }


    // GET PRODUCT BY ID

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(
                p => p.Id == id);
    }


    // GET PRODUCTS BY CATEGORY

    public async Task<List<Product>>
        GetByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .Include(p => p.Variants)
            .Where(p => p.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }


    // SEARCH PRODUCTS

    public async Task<List<Product>>
        SearchAsync(string query)
    {
        query = query.Trim();

        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .Include(p => p.Variants)
            .Where(p =>
                p.Name.Contains(query) ||
                p.Description.Contains(query) ||
                p.Category.Name.Contains(query) ||
                p.SubCategory.Name.Contains(query) ||
                p.Variants.Any(v =>
                    v.AgeGroup != null &&
                    v.AgeGroup.Contains(query)) ||
                p.Variants.Any(v =>
                    v.Size != null &&
                    v.Size.Contains(query))
            )
            .AsNoTracking()
            .ToListAsync();
    }


    // ADD PRODUCT

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);

        await _context.SaveChangesAsync();
    }


    // UPDATE PRODUCT

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);

        await _context.SaveChangesAsync();
    }


    // DELETE PRODUCT

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
    }


    // GET ALL PRODUCTS WITH PRICE SORTING

    public async Task<List<Product>>
        GetAllAsync(string? sort)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .Include(p => p.Variants)
            .AsNoTracking()
            .AsQueryable();

        if (sort == "price-low")
        {
            query = query
                .OrderBy(p => p.Price);
        }
        else if (sort == "price-high")
        {
            query = query
                .OrderByDescending(p => p.Price);
        }

        return await query.ToListAsync();
    }
}