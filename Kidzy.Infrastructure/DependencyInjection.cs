using Kidzy.Application.Interfaces;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Application.Services;

using Kidzy.Infrastructure.Authentication;
using Kidzy.Infrastructure.Data;
using Kidzy.Infrastructure.Data.Seed;
using Kidzy.Infrastructure.Payment;
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
        // DATABASE

        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection")));


        // REPOSITORIES
        

        services.AddScoped<
            IUserRepository,
            UserRepository>();

        services.AddScoped<
            ICategoryRepository,
            CategoryRepository>();

        services.AddScoped<
            IProductRepository,
            ProductRepository>();

        services.AddScoped<
            ICartRepository,
            CartRepository>();

        services.AddScoped<
            IWishlistRepository,
            WishlistRepository>();

        // User Order Repository
        services.AddScoped<
            IOrderRepository,
            OrderRepository>();

        // Admin Order Repository
        services.AddScoped<
            IAdminOrderRepository,
            AdminOrderRepository>();


        // APPLICATION SERVICES

        services.AddScoped<
            IPasswordHasher,
            PasswordHasher>();

        services.AddScoped<
            IJwtService,
            JwtService>();

        services.AddScoped<
            IAuthService,
            AuthService>();

        services.AddScoped<
            IUserService,
            UserService>();

        services.AddScoped<
            ICategoryService,
            CategoryService>();

        services.AddScoped<
            IProductService,
            ProductService>();

        services.AddScoped<
            ICartService,
            CartService>();

        services.AddScoped<
            IWishlistService,
            WishlistService>();

        // User Order Service
        services.AddScoped<
            IOrderService,
            OrderService>();

        // Admin Order Service
        services.AddScoped<
            IAdminOrderService,
            AdminOrderService>();


        // RAZORPAY

        services.AddHttpClient<
            IRazorpayService,
            RazorpayService>();


        // SEEDERS

        services.AddScoped<AdminSeeder>();


        return services;
    }
}