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

            // Product
            services.AddScoped<IProductService, ProductService>();

            // Cart
            services.AddScoped<ICartService, CartService>();

            // Wishlist
            services.AddScoped<IWishlistService, WishlistService>();

            // Order
            services.AddScoped<IOrderService, OrderService>();

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


            // User Repository
            services.AddScoped<
                IUserRepository,
                UserRepository>();

            // Category Repository
            services.AddScoped<
                ICategoryRepository,
                CategoryRepository>();

            // Product Repository
            services.AddScoped<
                IProductRepository,
                ProductRepository>();

            // Cart Repository
            services.AddScoped<
                ICartRepository,
                CartRepository>();

            // Wishlist Repository
            services.AddScoped<
                IWishlistRepository,
                WishlistRepository>();

            // Order Repository
            services.AddScoped<
                IOrderRepository,
                OrderRepository>();


            // Password Hasher
            services.AddScoped<
                IPasswordHasher,
                PasswordHasher>();

            // JWT Service
            services.AddScoped<
                IJwtService,
                JwtService>();

            return services;
        }
    }
}