using Kidzy.Application.Interfaces;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Application.Services;
using Kidzy.Infrastructure.Authentication;
using Kidzy.Infrastructure.Data;
using Kidzy.Infrastructure.Data.Seed;
using Kidzy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kidzy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection")));

        // Repositories
        services.AddScoped<
            IUserRepository,
            UserRepository>();

        // Authentication
        services.AddScoped<
            IPasswordHasher,
            PasswordHasher>();

        services.AddScoped<
            IJwtService,
            JwtService>();

        // Application Services
        services.AddScoped<
            IAuthService,
            AuthService>();

        services.AddScoped<
            IAdminService,
            AdminService>();

        // Seed
        services.AddScoped<AdminSeeder>();

        return services;
    }
}