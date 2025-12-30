using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WexChallenge.Application.DTOs;
using WexChallenge.Application.Services;

namespace WexChallenge.Controllers;

/// <summary>
/// API controller for managing purchase transactions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly IValidator<CreatePurchaseRequest> _validator;

    public PurchasesController(
        IPurchaseService purchaseService,
        IValidator<CreatePurchaseRequest> validator)
    {
        _purchaseService = purchaseService;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new purchase transaction.
    /// </summary>
    /// <param name="request">The purchase creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created purchase.</returns>
    /// <response code="201">Purchase created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePurchase(
        [FromBody] CreatePurchaseRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(
                validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ));
        }

        var result = await _purchaseService.CreatePurchaseAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetPurchase), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a purchase by its ID.
    /// </summary>
    /// <param name="id">The purchase ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The purchase details.</returns>
    /// <response code="200">Purchase found.</response>
    /// <response code="404">Purchase not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchase(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseService.GetPurchaseByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets all purchases.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all purchases.</returns>
    /// <response code="200">List of purchases.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PurchaseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPurchases(CancellationToken cancellationToken)
    {
        var result = await _purchaseService.GetAllPurchasesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a purchase converted to a specified currency.
    /// </summary>
    /// <param name="id">The purchase ID.</param>
    /// <param name="currency">The target currency (e.g., "Brazil-Real", "Euro Zone-Euro").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The purchase with currency conversion details.</returns>
    /// <response code="200">Purchase found and converted successfully.</response>
    /// <response code="404">Purchase not found.</response>
    /// <response code="422">Currency conversion not available within 6 months of purchase date.</response>
    [HttpGet("{id:guid}/convert")]
    [ProducesResponseType(typeof(ConvertedPurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetPurchaseInCurrency(
        Guid id,
        [FromQuery(Name = "currency")] string currency,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = "Currency parameter is required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var result = await _purchaseService.GetPurchaseInCurrencyAsync(id, currency, cancellationToken);
        return Ok(result);
    }
}
