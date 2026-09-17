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
        // APPLICATION SERVICES

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IWishlistService, WishlistService>();

            services.AddScoped<IOrderService, OrderService>();

            // Checkout Service
            services.AddScoped<ICheckoutService, CheckoutService>();

            return services;
        }


        // INFRASTRUCTURE SERVICES

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DATABASE

            services.AddDbContext<KidzyDbContext>(
                options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString(
                            "DefaultConnection")));


            // REPOSITORIES

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<IWishlistRepository, WishlistRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();


            // INFRASTRUCTURE SERVICES

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}