using Kidzy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kidzy.Infrastructure.Data.Configurations;

public class ReviewConfiguration
    : IEntityTypeConfiguration<Review>
{
    public void Configure(
        EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Rating
        builder.Property(x => x.Rating)
            .IsRequired();

        // Title
        builder.Property(x => x.Title)
            .HasMaxLength(100);

        // Comment
        builder.Property(x => x.Comment)
            .IsRequired()
            .HasMaxLength(1000);

        // CreatedAt
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Product relationship
        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // User relationship
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One user can review a product only once
        builder.HasIndex(x => new
        {
            x.UserId,
            x.ProductId
        })
        .IsUnique();
    }
}