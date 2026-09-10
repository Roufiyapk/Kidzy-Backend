using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Application.Services;
using Kidzy.Infrastructure.Data;
using Kidzy.Infrastructure.Repositories;
using Kidzy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Api.Extensions
{
    public static class DependencyInjection
    {
        // Application Services
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // Category
            services.AddScoped<ICategoryService, CategoryService>();

            // Product
            services.AddScoped<IProductService, ProductService>();

            return services;
        }

        // Infrastructure Services
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<KidzyDbContext>(
                options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString(
                            "DefaultConnection")));

            // User Repository
            services.AddScoped<IUserRepository, UserRepository>();

            // Category Repository
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Product Repository
            services.AddScoped<IProductRepository, ProductRepository>();

            // Password Hasher
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // JWT Service
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}