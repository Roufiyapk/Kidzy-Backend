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

    // GET USER BY EMAIL

    public async Task<User?> GetByEmailAsync(
        string email)
    {
        var normalizedEmail =
            email.Trim().ToLowerInvariant();

        return await _context.Users
            .FirstOrDefaultAsync(
                x =>
                    x.Email.ToLower()
                    == normalizedEmail);
    }

    // ADD USER

    public async Task<User> AddAsync(
        User user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }

    // GET ALL USERS

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    // GET USER BY ID

    public async Task<User?> GetByIdAsync(
        int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }

    // CHECK EMAIL EXISTS
    // excludeUserId = current user

    public async Task<bool> ExistsByEmailAsync(
        string email,
        int excludeUserId)
    {
        var normalizedEmail =
            email.Trim().ToLowerInvariant();

        return await _context.Users
            .AnyAsync(
                x =>
                    x.Id != excludeUserId &&
                    x.Email.ToLower()
                    == normalizedEmail);
    }

    // SAVE CHANGES

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}