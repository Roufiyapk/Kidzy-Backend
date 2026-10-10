using FluentValidation;
using Kidzy.Application.DTOs.Orders;

namespace Kidzy.Application.Validators.Orders;

public class CreateOrderValidator
    : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number must be exactly 10 digits.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters.");

        RuleFor(x => x.Place)
            .NotEmpty()
            .WithMessage("Place is required.")
            .MaximumLength(200)
            .WithMessage("Place cannot exceed 200 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required.")
            .MaximumLength(100)
            .WithMessage("State cannot exceed 100 characters.");

        RuleFor(x => x.Pincode)
            .NotEmpty()
            .WithMessage("Pincode is required.")
            .Matches(@"^\d{6}$")
            .WithMessage("Pincode must be exactly 6 digits.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required.")
            .Must(IsValidPaymentMethod)
            .WithMessage("Payment method must be cod or razorpay.");

        When(
            x =>
                !string.IsNullOrWhiteSpace(x.PaymentMethod) &&
                x.PaymentMethod.Trim().ToLowerInvariant() == "razorpay",
            () =>
            {
                RuleFor(x => x.PaymentId)
                    .NotNull()
                    .WithMessage("Verified payment is required.");
            });

        When(
            x =>
                !string.IsNullOrWhiteSpace(x.PaymentMethod) &&
                x.PaymentMethod.Trim().ToLowerInvariant() == "cod",
            () =>
            {
                RuleFor(x => x.PaymentId)
                    .Null()
                    .WithMessage("COD orders cannot contain a payment ID.");
            });
    }

    private static bool IsValidPaymentMethod(
        string paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            return false;
        }

        var method =
            paymentMethod
                .Trim()
                .ToLowerInvariant();

        return method == "cod"
            || method == "razorpay";
    }
}