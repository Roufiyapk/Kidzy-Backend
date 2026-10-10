using FluentValidation;

using Kidzy.Application.DTOs.Orders;

namespace Kidzy.Application.Validators.Orders;

public class BuyNowOrderValidator
    : AbstractValidator<BuyNowOrderDto>
{
    public BuyNowOrderValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage(
                "Product is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(
                "Quantity must be greater than zero.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(
                "Name is required.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage(
                "Address is required.");

        RuleFor(x => x.Place)
            .NotEmpty()
            .WithMessage(
                "Place is required.");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage(
                "State is required.");

        RuleFor(x => x.Pincode)
            .NotEmpty()
            .WithMessage(
                "Pincode is required.")
            .Matches(@"^\d{6}$")
            .WithMessage(
                "Pincode must be exactly 6 digits.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(
                "Phone number is required.")
            .Matches(@"^\d{10}$")
            .WithMessage(
                "Phone number must be exactly 10 digits.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage(
                "Payment method is required.")
            .Must(
                x =>
                    x.Trim()
                        .ToLowerInvariant() == "cod" ||
                    x.Trim()
                        .ToLowerInvariant() == "razorpay")
            .WithMessage(
                "Invalid payment method.");
    }
}