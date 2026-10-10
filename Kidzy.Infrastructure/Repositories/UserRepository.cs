using Kidzy.Application.DTOs.User;
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

    // GET USERS
   

    public async Task<List<UserListDto>> GetUsersAsync(
        string? search,
        string? status,
        int page,
        int pageSize)
    {
        var query = _context.Users
            .AsNoTracking()
            .Where(
                x =>
                    x.Role.ToString() != "Admin")
            .AsQueryable();

        // SEARCH

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim()
                    .ToLowerInvariant();

            query = query.Where(
                x =>
                    x.Name
                        .ToLower()
                        .Contains(searchTerm)

                    ||

                    x.Email
                        .ToLower()
                        .Contains(searchTerm)

                    ||

                    (
                        x.Phone != null &&
                        x.Phone
                            .ToLower()
                            .Contains(searchTerm)
                    ));
        }

        // STATUS FILTER

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus =
                status.Trim()
                    .ToLowerInvariant();

            if (normalizedStatus == "active")
            {
                query = query.Where(
                    x => !x.IsBlocked);
            }
            else if (normalizedStatus == "blocked")
            {
                query = query.Where(
                    x => x.IsBlocked);
            }
        }

        // SORT + PAGINATION + DTO PROJECTION

        return await query
            .OrderBy(x => x.Name)

            .Skip(
                (page - 1) *
                pageSize)

            .Take(pageSize)

            .Select(
                x => new UserListDto
                {
                    Id = x.Id,

                    Name = x.Name,

                    Email = x.Email,

                    Phone = x.Phone,

                    Address = x.Address,

                    Pincode = x.Pincode,

                    Role =
                        x.Role.ToString(),

                    IsBlocked =
                        x.IsBlocked
                })

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

    public async Task<bool> ExistsByEmailAsync(
        string email,
        int excludeUserId)
    {
        var normalizedEmail =
            email.Trim()
                .ToLowerInvariant();

        return await _context.Users
            .AnyAsync(
                x =>
                    x.Id != excludeUserId
                    &&
                    x.Email.ToLower()
                        == normalizedEmail);
    }

    // SAVE CHANGES

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}