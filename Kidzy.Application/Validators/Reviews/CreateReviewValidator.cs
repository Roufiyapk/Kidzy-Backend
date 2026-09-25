using FluentValidation;
using Kidzy.Application.DTOs.Reviews;

namespace Kidzy.Application.Validators.Reviews;

public class CreateReviewValidator
    : AbstractValidator<CreateReviewDto>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage(
                "Rating must be between 1 and 5.");

        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage(
                "Review title cannot exceed 100 characters.")
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Comment)
            .NotEmpty()
            .WithMessage(
                "Review comment is required.")
            .MaximumLength(1000)
            .WithMessage(
                "Review comment cannot exceed 1000 characters.");
    }
}