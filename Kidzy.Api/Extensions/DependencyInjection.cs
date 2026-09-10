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
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // Category
            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<KidzyDbContext>(
                options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString(
                            "DefaultConnection")));

            // User
            services.AddScoped<
                IUserRepository,
                UserRepository>();

            // Category
            services.AddScoped<
                ICategoryRepository,
                CategoryRepository>();

            // Password
            services.AddScoped<
                IPasswordHasher,
                PasswordHasher>();

            // JWT
            services.AddScoped<
                IJwtService,
                JwtService>();

            return services;
        }
    }
}