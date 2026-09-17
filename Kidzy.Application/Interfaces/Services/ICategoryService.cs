using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);
    }
}