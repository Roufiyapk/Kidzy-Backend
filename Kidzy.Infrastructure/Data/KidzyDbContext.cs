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

        // Wishlist
        public DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<WishlistItem> WishlistItems { get; set; }


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


            // Cart → CartItems
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);


            // User → Cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Wishlist → WishlistItems
            modelBuilder.Entity<Wishlist>()
                .HasMany(w => w.WishlistItems)
                .WithOne(wi => wi.Wishlist)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);


            // User → Wishlist
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Product → WishlistItems
            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            // One wishlist per user
            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => w.UserId)
                .IsUnique();


            // Same product should not be added twice
            modelBuilder.Entity<WishlistItem>()
                .HasIndex(wi => new
                {
                    wi.WishlistId,
                    wi.ProductId
                })
                .IsUnique();
        }
    }
}