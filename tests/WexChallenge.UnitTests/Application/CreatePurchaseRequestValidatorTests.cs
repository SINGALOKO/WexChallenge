using FluentAssertions;
using FluentValidation.TestHelper;
using WexChallenge.Application.DTOs;
using WexChallenge.Application.Validators;

namespace WexChallenge.UnitTests.Application;

public class CreatePurchaseRequestValidatorTests
{
    private readonly CreatePurchaseRequestValidator _validator;

    public CreatePurchaseRequestValidatorTests()
    {
        _validator = new CreatePurchaseRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = "Test Purchase",
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 100.50m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyDescription_ShouldHaveError()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = "",
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 100.50m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithDescriptionExceeding50Characters_ShouldHaveError()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = new string('a', 51),
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 100.50m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_WithDescription50Characters_ShouldNotHaveError()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = new string('a', 50),
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 100.50m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithZeroAmount_ShouldHaveError()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = "Test",
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 0m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountInUsd)
            .WithErrorMessage("Purchase amount must be a positive value.");
    }

    [Fact]
    public void Validate_WithNegativeAmount_ShouldHaveError()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = "Test",
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = -50m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountInUsd);
    }

    [Fact]
    public void Validate_WithFutureDate_ShouldHaveError()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            Description = "Test",
            TransactionDate = DateTime.UtcNow.Date.AddDays(7),
            AmountInUsd = 100m
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TransactionDate)
            .WithErrorMessage("Transaction date cannot be in the future.");
    }
}
