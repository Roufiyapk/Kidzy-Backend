using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kidzy.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }

    // Create Admin automatically
    public async Task SeedAdminAsync()
    {
        var adminName =
            _configuration["Admin:Name"];

        var adminEmail =
            _configuration["Admin:Email"];

        var adminPassword =
            _configuration["Admin:Password"];

        // Check configuration
        if (string.IsNullOrWhiteSpace(adminName) ||
            string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        adminEmail =
            adminEmail.Trim().ToLowerInvariant();

        // Check whether Admin already exists
        var existingAdmin =
            await Users.FirstOrDefaultAsync(
                u => u.Email == adminEmail);

        if (existingAdmin != null)
        {
            return;
        }

        // Password hashing
        var passwordHasher =
            new PasswordHasher<User>();

        var admin = new User
        {
            Name = adminName.Trim(),

            Email = adminEmail,

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

        await Users.AddAsync(admin);

        await SaveChangesAsync();
    }
}