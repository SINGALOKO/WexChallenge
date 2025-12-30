using WexChallenge.Application.DTOs;
using WexChallenge.Domain.Entities;
using WexChallenge.Domain.Exceptions;
using WexChallenge.Domain.Interfaces;

namespace WexChallenge.Application.Services;

/// <summary>
/// Application service implementation for purchase operations.
/// </summary>
public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IExchangeRateService _exchangeRateService;

    public PurchaseService(
        IPurchaseRepository purchaseRepository,
        IExchangeRateService exchangeRateService)
    {
        _purchaseRepository = purchaseRepository;
        _exchangeRateService = exchangeRateService;
    }

    /// <inheritdoc />
    public async Task<PurchaseResponse> CreatePurchaseAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default)
    {
        var purchase = Purchase.Create(
            request.Description,
            request.TransactionDate,
            request.AmountInUsd);

        await _purchaseRepository.AddAsync(purchase, cancellationToken);

        return MapToResponse(purchase);
    }

    /// <inheritdoc />
    public async Task<PurchaseResponse> GetPurchaseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(id, cancellationToken);

        if (purchase is null)
            throw new PurchaseNotFoundException(id);

        return MapToResponse(purchase);
    }

    /// <inheritdoc />
    public async Task<ConvertedPurchaseResponse> GetPurchaseInCurrencyAsync(Guid id, string currency, CancellationToken cancellationToken = default)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(id, cancellationToken);

        if (purchase is null)
            throw new PurchaseNotFoundException(id);

        var exchangeRate = await _exchangeRateService.GetExchangeRateAsync(
            currency,
            purchase.TransactionDate,
            cancellationToken);

        if (exchangeRate is null)
            throw new ExchangeRateUnavailableException(currency, purchase.TransactionDate);

        var convertedAmount = exchangeRate.ConvertFromUsd(purchase.AmountInUsd);

        return new ConvertedPurchaseResponse
        {
            Id = purchase.Id,
            Description = purchase.Description,
            TransactionDate = purchase.TransactionDate,
            OriginalAmountInUsd = purchase.AmountInUsd,
            TargetCurrency = exchangeRate.Currency,
            ExchangeRate = exchangeRate.Rate,
            ExchangeRateDate = exchangeRate.EffectiveDate,
            ConvertedAmount = convertedAmount
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PurchaseResponse>> GetAllPurchasesAsync(CancellationToken cancellationToken = default)
    {
        var purchases = await _purchaseRepository.GetAllAsync(cancellationToken);
        return purchases.Select(MapToResponse).ToList();
    }

    private static PurchaseResponse MapToResponse(Purchase purchase)
    {
        return new PurchaseResponse
        {
            Id = purchase.Id,
            Description = purchase.Description,
            TransactionDate = purchase.TransactionDate,
            AmountInUsd = purchase.AmountInUsd
        };
    }
}
