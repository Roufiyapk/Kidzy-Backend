using Kidzy.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PaymentEntity = Kidzy.Domain.Entities.Payment;

namespace Kidzy.Infrastructure.Data.Configurations;

public class PaymentConfiguration
    : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(
        EntityTypeBuilder<PaymentEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.RazorpayOrderId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.RazorpayPaymentId)
            .HasMaxLength(200);

        builder.Property(x => x.RazorpaySignature)
            .HasMaxLength(500);

        builder.Property(x => x.Receipt)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CheckoutType)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.ProductVariantId)
            .IsRequired(false);

        builder.Property(x => x.ProductId)
            .IsRequired(false);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.VerifiedAt)
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.RazorpayOrderId)
            .IsUnique();
    }
}