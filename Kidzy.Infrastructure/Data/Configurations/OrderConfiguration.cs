using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kidzy.Infrastructure.Data.Configurations;

public class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(
        EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Subtotal)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.DeliveryFee)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ShippingName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ShippingPincode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.PaymentStatus)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.RazorpayPaymentId)
            .HasMaxLength(200);

        builder.Property(x => x.RazorpayOrderId)
            .HasMaxLength(200);

        builder.Property(x => x.RazorpaySignature)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(
                OrderStatus.OrderPlaced);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}