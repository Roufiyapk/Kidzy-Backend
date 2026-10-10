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

        services.AddScoped<
            IUserRepository,
            UserRepository>();

        services.AddScoped<
            IProductRepository,
            ProductRepository>();

        services.AddScoped<
            ICartRepository,
            CartRepository>();

        services.AddScoped<
            ICategoryRepository,
            CategoryRepository>();

        services.AddScoped<
            IWishlistRepository,
            WishlistRepository>();

        services.AddScoped<
            IOrderRepository,
            OrderRepository>();

        services.AddScoped<
            IPaymentRepository,
            PaymentRepository>();

        services.AddScoped<
            IReviewRepository,
            ReviewRepository>();

        services.AddScoped<
            IDashboardRepository,
            DashboardRepository>();


        // AUTHENTICATION

        services.AddScoped<
            IPasswordHasher,
            PasswordHasher>();

        services.AddScoped<
            IJwtService,
            JwtService>();


        // APPLICATION SERVICES

        services.AddScoped<
            IAuthService,
            AuthService>();

        services.AddScoped<
            IUserService,
            UserService>();

        services.AddScoped<
            IProductService,
            ProductService>();

        services.AddScoped<
            ICategoryService,
            CategoryService>();

        services.AddScoped<
            ICartService,
            CartService>();

        services.AddScoped<
            IWishlistService,
            WishlistService>();

        services.AddScoped<
            IOrderService,
            OrderService>();

        services.AddScoped<
            IPaymentService,
            PaymentService>();

        services.AddScoped<
            IReviewService,
            ReviewService>();

        services.AddScoped<
            IDashboardService,
            DashboardService>();


        // CLOUDINARY

        services.AddScoped<
            ICloudinaryService,
            CloudinaryService>();


        // RAZORPAY

        services.AddScoped<
            IRazorpayService,
            RazorpayService>();


        // ADMIN SEEDER

        services.AddScoped<
            AdminSeeder>();


        return services;
    }
}