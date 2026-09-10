using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<Category> CreateAsync(Category category);

        Task<bool> DeleteAsync(int id);
    }
}