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
            .WithMessage("Invalid product.");

        RuleFor(x => x.ProductVariantId)
            .GreaterThan(0)
            .When(x => x.ProductVariantId.HasValue)
            .WithMessage("Invalid product variant.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(
                "Quantity must be greater than zero.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage(
                "Name cannot exceed 100 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\d{10}$")
            .WithMessage(
                "Phone number must be exactly 10 digits.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(500)
            .WithMessage(
                "Address cannot exceed 500 characters.");

        RuleFor(x => x.Pincode)
            .NotEmpty()
            .WithMessage("Pincode is required.")
            .Matches(@"^\d{6}$")
            .WithMessage(
                "Pincode must be exactly 6 digits.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required.")
            .Must(IsValidPaymentMethod)
            .WithMessage(
                "Payment method must be cod or razorpay.");

        When(
            x => x.PaymentMethod
                .Trim()
                .ToLowerInvariant()
                == "razorpay",
            () =>
            {
                RuleFor(x => x)
                    .Must(HasEitherNoPaymentDetailsOrAll)
                    .WithMessage(
                        "Razorpay payment details are incomplete.");
            });
    }

    private static bool IsValidPaymentMethod(
        string paymentMethod)
    {
        var method =
            paymentMethod
                .Trim()
                .ToLowerInvariant();

        return method == "cod"
            || method == "razorpay";
    }

    private static bool
        HasEitherNoPaymentDetailsOrAll(
            BuyNowOrderDto dto)
    {
        var hasOrderId =
            !string.IsNullOrWhiteSpace(
                dto.RazorpayOrderId);

        var hasPaymentId =
            !string.IsNullOrWhiteSpace(
                dto.RazorpayPaymentId);

        var hasSignature =
            !string.IsNullOrWhiteSpace(
                dto.RazorpaySignature);

        if (!hasOrderId &&
            !hasPaymentId &&
            !hasSignature)
        {
            return true;
        }

        return hasOrderId &&
               hasPaymentId &&
               hasSignature;
    }
}