using FluentValidation;
using Kidzy.Application.DTOs.Profile;

namespace Kidzy.Application.Validators.Profile;

public class UpdateProfileValidator
    : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileValidator()
    {
        // Name
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage(
                "Name cannot exceed 100 characters.");

        // Email
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage(
                "Enter a valid email address.")
            .MaximumLength(150)
            .WithMessage(
                "Email cannot exceed 150 characters.");

        // Phone
        RuleFor(x => x.Phone)
            .Matches(@"^\d{10}$")
            .WithMessage(
                "Phone number must be exactly 10 digits.")
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Phone));

        // Pincode
        RuleFor(x => x.Pincode)
            .Matches(@"^\d{6}$")
            .WithMessage(
                "PIN code must be exactly 6 digits.")
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Pincode));

        // Address -> Pincode required
        RuleFor(x => x.Pincode)
            .NotEmpty()
            .WithMessage(
                "PIN code is required when address is provided.")
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Address));
    }
}