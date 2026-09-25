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
        // Fixed admin details
        const string adminName = "Admin";
        const string adminEmail = "admin@kidzy.com";

        // Admin password comes from User Secrets
        var adminPassword =
            _configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var normalizedEmail =
            adminEmail.Trim().ToLowerInvariant();

        // Check whether admin user already exists
        var existingUser =
            await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == normalizedEmail);

        var passwordHasher =
            new PasswordHasher<User>();

        // =====================================================
        // USER ALREADY EXISTS
        // =====================================================

        if (existingUser != null)
        {
            bool changed = false;

            // Make sure this account is Admin
            if (existingUser.Role != UserRole.Admin)
            {
                existingUser.Role = UserRole.Admin;
                changed = true;
            }

            // Make sure admin is not blocked
            if (existingUser.IsBlocked)
            {
                existingUser.IsBlocked = false;
                changed = true;
            }

            // Make sure the configured admin password works
            var passwordResult =
                passwordHasher.VerifyHashedPassword(
                    existingUser,
                    existingUser.PasswordHash,
                    adminPassword);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                existingUser.PasswordHash =
                    passwordHasher.HashPassword(
                        existingUser,
                        adminPassword);

                changed = true;
            }

            if (!string.Equals(
                    existingUser.Name,
                    adminName,
                    StringComparison.Ordinal))
            {
                existingUser.Name = adminName;
                changed = true;
            }

            if (changed)
            {
                await _context.SaveChangesAsync();
            }

            return;
        }

        // =====================================================
        // CREATE ADMIN IF NOT EXISTS
        // =====================================================

        var admin = new User
        {
            Name = adminName,

            Email = normalizedEmail,

            PasswordHash =
                passwordHasher.HashPassword(
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