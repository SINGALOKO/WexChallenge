using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WexChallenge.Application.Services;
using WexChallenge.Application.Validators;

namespace WexChallenge.Application;

/// <summary>
/// Extension methods for configuring Application layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the service collection.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register validators from this assembly
        services.AddValidatorsFromAssemblyContaining<CreatePurchaseRequestValidator>();

        // Register application services
        services.AddScoped<IPurchaseService, PurchaseService>();

        return services;
    }
}
