using Kidzy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Data
{
    public class KidzyDbContext : DbContext
    {
        public KidzyDbContext(
            DbContextOptions<KidzyDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductSize> ProductSizes { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product Price
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Category → Products
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product → ProductSizes
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductSizes)
                .WithOne(ps => ps.Product)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → Cart
            modelBuilder.Entity<User>()
                .HasOne<Cart>()
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One Cart per User
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            // Cart → CartItems
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product → CartItems
            modelBuilder.Entity<Product>()
                .HasMany<CartItem>()
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate Product + Size in same cart
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new
                {
                    ci.CartId,
                    ci.ProductId,
                    ci.SelectedSize
                })
                .IsUnique();
        }
    }
}