using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kidzy.Infrastructure.Data.Seed;

public class AdminSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AdminSeeder(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        var adminName = _configuration["Admin:Name"];
        var adminEmail = _configuration["Admin:Email"];
        var adminPassword = _configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminName) ||
            string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        adminEmail = adminEmail.Trim().ToLowerInvariant();

        var existingAdmin = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingAdmin != null)
        {
            return;
        }

        var passwordHasher = new PasswordHasher<User>();

        var admin = new User
        {
            Name = adminName.Trim(),
            Email = adminEmail,

            PasswordHash = passwordHasher.HashPassword(
                null!,
                adminPassword),

            Role = UserRole.Admin,

            IsBlocked = false,

            Phone = null,
            Address = null,
            Pincode = null
        };

        await _context.Users.AddAsync(admin);

        await _context.SaveChangesAsync();
    }
}