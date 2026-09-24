using Kidzy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kidzy.Infrastructure.Data.Configurations;

public class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(
        EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.ProductId)
            .IsRequired();


        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(150);


        builder.Property(x => x.AgeGroup)
            .HasMaxLength(50);


        builder.Property(x => x.Size)
            .HasMaxLength(20);


        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");


        builder.Property(x => x.Quantity)
            .IsRequired();


        builder.Property(x => x.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");


        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(
                DeleteBehavior.Cascade);


        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(
                DeleteBehavior.Restrict);


        builder.HasOne(x => x.ProductVariant)
            .WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(
                DeleteBehavior.Restrict);
    }
}