using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using WexChallenge.Domain.Interfaces;
using WexChallenge.Domain.ValueObjects;
using WexChallenge.Infrastructure.Persistence;

namespace WexChallenge.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration tests with in-memory database and mocked external services.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IExchangeRateService MockExchangeRateService { get; } = Substitute.For<IExchangeRateService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Remove all DbContext related registrations
            var descriptorsToRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
            });

            // Remove and replace the exchange rate service
            services.RemoveAll<IExchangeRateService>();
            services.AddSingleton(MockExchangeRateService);

            // Configure default mock behavior
            MockExchangeRateService
                .GetExchangeRateAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
                .Returns(new ExchangeRate("Brazil", "Real", 5.00m, DateTime.UtcNow.Date));
        });
    }
}
