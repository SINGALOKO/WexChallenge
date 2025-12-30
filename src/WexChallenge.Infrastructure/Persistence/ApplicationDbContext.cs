using Microsoft.EntityFrameworkCore;
using WexChallenge.Domain.Entities;

namespace WexChallenge.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the application.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public DbSet<Purchase> Purchases => Set<Purchase>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.TransactionDate)
                .IsRequired();

            entity.Property(e => e.AmountInUsd)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });
    }
}
