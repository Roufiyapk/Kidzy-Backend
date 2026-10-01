using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Application.Services;

using Kidzy.Infrastructure.Authentication;
using Kidzy.Infrastructure.Cloudinary;
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

        // User
        services.AddScoped<
            IUserRepository,
            UserRepository>();



        // Product
        services.AddScoped<
            IProductRepository,
            ProductRepository>();


        // Cart
        services.AddScoped<
            ICartRepository,


            CartRepository>();


        // Category 

        services.AddScoped<
            ICategoryRepository,
            CategoryRepository>();


        // Wishlist
        services.AddScoped<
            IWishlistRepository,
            WishlistRepository>();


        // ORDER
        // User + Admin

        services.AddScoped<
            IOrderRepository,
            OrderRepository>();


        // Review
        services.AddScoped<
            IReviewRepository,
            ReviewRepository>();


        // APPLICATION SERVICES

        // Password
        services.AddScoped<
            IPasswordHasher,
            PasswordHasher>();


        // JWT
        services.AddScoped<
            IJwtService,
            JwtService>();


        // Authentication
        services.AddScoped<
            IAuthService,
            AuthService>();


        // User
        services.AddScoped<
            IUserService,
            UserService>();




        // Product
        services.AddScoped<
            IProductService,
            ProductService>();


        // Cart
        services.AddScoped<
            ICartService,
            CartService>();


        // Wishlist
        services.AddScoped<
            IWishlistService,
            WishlistService>();


        // ORDER
        // User + Admin

        services.AddScoped<
            IOrderService,
            OrderService>();


        // Review
        services.AddScoped<
            IReviewService,
            ReviewService>();


        // CLOUDINARY

        services.AddScoped<
            ICloudinaryService,
            CloudinaryService>();


        // RAZORPAY

        services.AddHttpClient<
            IRazorpayService,
            RazorpayService>();


        // SEEDERS

        services.AddScoped<
            AdminSeeder>();


        return services;
    }
}