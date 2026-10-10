using Kidzy.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kidzy.Infrastructure.Data.Configurations;

public class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(
        EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(
            x => x.Id);


        builder.Property(
                x => x.Subtotal)
            .HasPrecision(
                18,
                2);


        builder.Property(
                x => x.DeliveryFee)
            .HasPrecision(
                18,
                2);


        builder.Property(
                x => x.TotalAmount)
            .HasPrecision(
                18,
                2);


        builder.Property(
                x => x.ShippingName)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(
                x => x.ShippingPhone)
            .IsRequired()
            .HasMaxLength(20);


        builder.Property(
                x => x.ShippingAddress)
            .IsRequired()
            .HasMaxLength(1000);


        builder.Property(
                x => x.ShippingPlace)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(
                x => x.ShippingState)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(
                x => x.ShippingPincode)
            .IsRequired()
            .HasMaxLength(10);


        builder.Property(
                x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);


        builder.Property(
                x => x.PaymentStatus)
            .IsRequired()
            .HasMaxLength(50);


        builder.Property(
                x => x.RazorpayPaymentId)
            .HasMaxLength(200);


        builder.Property(
                x => x.RazorpayOrderId)
            .HasMaxLength(200);


        builder.Property(
                x => x.RazorpaySignature)
            .HasMaxLength(500);


        // USER
        builder.HasOne(
                x => x.User)
            .WithMany()
            .HasForeignKey(
                x => x.UserId)
            .OnDelete(
                DeleteBehavior.Restrict);


        // PAYMENT
        builder.HasOne(
                x => x.Payment)
            .WithOne(
                x => x.Order)
            .HasForeignKey<Order>(
                x => x.PaymentId)
            .OnDelete(
                DeleteBehavior.Restrict);


        // ITEMS
        builder.HasMany(
                x => x.Items)
            .WithOne(
                x => x.Order)
            .HasForeignKey(
                x => x.OrderId)
            .OnDelete(
                DeleteBehavior.Cascade);
    }
}