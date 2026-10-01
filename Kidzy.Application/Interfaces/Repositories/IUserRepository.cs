using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User> AddAsync(User user);

    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<bool> ExistsByEmailAsync(
        string email,
        int excludeUserId);

    Task SaveChangesAsync();
}