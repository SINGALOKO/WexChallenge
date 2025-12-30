using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using WexChallenge.Domain.Interfaces;
using WexChallenge.Infrastructure.ExternalServices;
using WexChallenge.Infrastructure.Persistence;

namespace WexChallenge.Infrastructure;

/// <summary>
/// Extension methods for configuring Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">SQLite connection string. If null, uses in-memory database.</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString = null)
    {
        // Configure Entity Framework Core with SQLite
        if (string.IsNullOrEmpty(connectionString))
        {
            // Use SQLite with a file-based database by default
            connectionString = "Data Source=wexchallenge.db";
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register repositories
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        // Configure HttpClient for Treasury API with Polly resilience policies
        services.AddHttpClient<IExchangeRateService, TreasuryExchangeRateService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "WexChallenge/1.0");
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Add Memory Cache
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Retry policy: retry 3 times with exponential backoff.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    /// <summary>
    /// Circuit breaker policy: break circuit after 5 consecutive failures for 30 seconds.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
