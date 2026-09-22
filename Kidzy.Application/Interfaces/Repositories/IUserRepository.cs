using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<IEnumerable<User>> GetAllAsync();

    Task<User> AddAsync(User user);

    Task UpdateAsync(User user);
}