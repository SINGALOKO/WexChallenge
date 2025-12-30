using Microsoft.EntityFrameworkCore;
using WexChallenge.Domain.Entities;
using WexChallenge.Domain.Interfaces;

namespace WexChallenge.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core implementation of the purchase repository.
/// </summary>
public class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        await _context.Purchases.AddAsync(purchase, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .AsNoTracking()
            .OrderByDescending(p => p.TransactionDate)
            .ToListAsync(cancellationToken);
    }
}
