using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
        string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(
        int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.Id == id);
    }

    public async Task<IEnumerable<User>>
        GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User> AddAsync(
        User user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task UpdateAsync(
        User user)
    {
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
    }
}