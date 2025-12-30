using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WexChallenge.Application.DTOs;

namespace WexChallenge.IntegrationTests;

public class PurchasesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PurchasesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreatePurchase_WithValidData_ShouldReturn201Created()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new CreatePurchaseRequest
        {
            Description = "Integration Test Purchase",
            TransactionDate = DateTime.UtcNow.Date.AddDays(-1),
            AmountInUsd = 150.75m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/purchases", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<PurchaseResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBe(Guid.Empty);
        result.Description.Should().Be(request.Description);
        result.AmountInUsd.Should().Be(request.AmountInUsd);
    }

    [Fact]
    public async Task CreatePurchase_WithDescriptionExceeding50Characters_ShouldReturn400BadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new CreatePurchaseRequest
        {
            Description = new string('a', 51),
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 100m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/purchases", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePurchase_WithNegativeAmount_ShouldReturn400BadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new CreatePurchaseRequest
        {
            Description = "Test",
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = -50m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/purchases", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAndGetPurchase_ShouldReturnSamePurchase()
    {
        // Arrange - Create a purchase
        using var client = _factory.CreateClient();
        var uniqueDescription = "Get Test Purchase " + Guid.NewGuid();
        var createRequest = new CreatePurchaseRequest
        {
            Description = uniqueDescription,
            TransactionDate = DateTime.UtcNow.Date,
            AmountInUsd = 200m
        };
        
        var createResponse = await client.PostAsJsonAsync("/api/purchases", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPurchase = await createResponse.Content.ReadFromJsonAsync<PurchaseResponse>();
        createdPurchase.Should().NotBeNull();

        // Act - Get the same purchase by ID
        var getResponse = await client.GetAsync($"/api/purchases/{createdPurchase!.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await getResponse.Content.ReadFromJsonAsync<PurchaseResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdPurchase.Id);
        result.Description.Should().Be(uniqueDescription);
    }

    [Fact]
    public async Task GetPurchase_WithNonExistentId_ShouldReturn404NotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/purchases/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllPurchases_ShouldReturn200Ok()
    {
        // Arrange
        using var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/purchases");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
