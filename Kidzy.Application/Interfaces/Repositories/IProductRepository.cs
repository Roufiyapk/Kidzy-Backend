using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product> CreateAsync(Product product);

        Task<bool> DeleteAsync(int id);
    }
}