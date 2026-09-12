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
            // Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICheckoutService, CheckoutService>();

            return services;
        }

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

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Infrastructure Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}