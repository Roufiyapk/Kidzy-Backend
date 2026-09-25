using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<List<Product>> GetByCategoryAsync(
        int categoryId);

    Task<List<Product>> SearchAsync(
        string query);

    Task AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task DeleteAsync(Product product);
}