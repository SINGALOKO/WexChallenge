using FluentValidation;
using WexChallenge.Application.DTOs;

namespace WexChallenge.Application.Validators;

/// <summary>
/// Validator for CreatePurchaseRequest using FluentValidation.
/// </summary>
public class CreatePurchaseRequestValidator : AbstractValidator<CreatePurchaseRequest>
{
    public CreatePurchaseRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(50)
            .WithMessage("Description must not exceed 50 characters.");

        RuleFor(x => x.TransactionDate)
            .NotEmpty()
            .WithMessage("Transaction date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1))
            .WithMessage("Transaction date cannot be in the future.");

        RuleFor(x => x.AmountInUsd)
            .GreaterThan(0)
            .WithMessage("Purchase amount must be a positive value.")
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
            .WithMessage("Purchase amount must be rounded to the nearest cent (2 decimal places).");
    }
}
