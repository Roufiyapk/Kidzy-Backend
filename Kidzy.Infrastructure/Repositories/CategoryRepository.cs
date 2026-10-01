using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // GET ALL CATEGORIES

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.SubCategories)
            .AsNoTracking()
            .ToListAsync();
    }


    // GET CATEGORY BY ID

    public async Task<Category?> GetByIdAsync(
        int categoryId)
    {
        return await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(
                c => c.Id == categoryId);
    }


    // CHECK CATEGORY EXISTS

    public async Task<bool> ExistsAsync(
        int categoryId)
    {
        return await _context.Categories
            .AnyAsync(
                c => c.Id == categoryId);
    }
}