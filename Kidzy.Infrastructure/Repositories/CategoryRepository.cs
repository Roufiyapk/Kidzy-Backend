using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly KidzyDbContext _context;

        public CategoryRepository(
            KidzyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}