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
        }
    }
}