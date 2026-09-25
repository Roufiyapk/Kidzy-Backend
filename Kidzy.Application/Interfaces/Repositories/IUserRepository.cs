using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IUserRepository
{
    // Authentication
    Task<User?> GetByEmailAsync(string email);

    Task<User> AddAsync(User user);

    // Admin
    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    // Profile
    Task<bool> ExistsByEmailAsync(
        string email,
        int excludeUserId);

    // Save
    Task SaveChangesAsync();
}