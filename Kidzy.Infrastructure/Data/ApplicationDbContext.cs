using Kidzy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<SubCategory> SubCategories { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;

    public DbSet<Cart> Carts { get; set; } = null!;

    public DbSet<CartItem> CartItems { get; set; } = null!;

    public DbSet<Wishlist> Wishlists { get; set; } = null!;

    public DbSet<WishlistItem> WishlistItems { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;

    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}