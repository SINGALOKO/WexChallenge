# WexChallenge - Purchase Transaction API

A .NET 9 Web API application for managing purchase transactions with currency conversion support using the U.S. Treasury Reporting Rates of Exchange API.

## ?? Requirements

This application implements:
1. **Store a Purchase Transaction** - Accept and persist purchase transactions with description, date, and USD amount
2. **Retrieve in Specified Currency** - Convert purchases to currencies supported by the Treasury API

## ??? Architecture

This project follows **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability.

```
WexChallenge/
??? src/
?   ??? WexChallenge.Domain/          # Core business entities and interfaces
?   ?   ??? Entities/                  # Purchase entity
?   ?   ??? ValueObjects/              # ExchangeRate value object
?   ?   ??? Exceptions/                # Domain-specific exceptions
?   ?   ??? Interfaces/                # Repository and service contracts
?   ?
?   ??? WexChallenge.Application/      # Application logic and use cases
?   ?   ??? DTOs/                      # Request/Response objects
?   ?   ??? Validators/                # FluentValidation validators
?   ?   ??? Services/                  # Application services
?   ?
?   ??? WexChallenge.Infrastructure/   # External dependencies
?       ??? Persistence/               # EF Core DbContext and repositories
?       ??? ExternalServices/          # Treasury API client
?
??? WexChallenge/                      # Web API layer
?   ??? Controllers/                   # REST API endpoints
?   ??? Middleware/                    # Global exception handling
?
??? tests/
    ??? WexChallenge.UnitTests/        # Unit tests
    ??? WexChallenge.IntegrationTests/ # Integration tests
```

### Why Clean Architecture?

- **Independence from frameworks**: Business logic doesn't depend on ASP.NET Core
- **Testability**: Each layer can be tested in isolation
- **Independence from UI**: The same business logic could be used with different interfaces
- **Independence from database**: SQLite can be swapped without affecting business logic
- **Independence from external agencies**: Treasury API changes don't affect core business rules

## ?? Technical Decisions

### Persistence (SQLite)
- **Why SQLite?** No external database installation required, demonstrates knowledge of EF Core migrations, real file-based persistence
- **Alternative considered**: In-Memory Database (used only for testing)

### External API Resilience (Polly)
- **Retry Policy**: 3 retries with exponential backoff (2s, 4s, 8s)
- **Circuit Breaker**: Opens after 5 consecutive failures, stays open for 30 seconds
- **IHttpClientFactory**: Prevents socket exhaustion

### Caching Strategy
- **MemoryCache** for exchange rates: Historical rates don't change, 24-hour cache duration
- Reduces load on Treasury API
- Improves response times for repeated conversions

### Validation (FluentValidation)
- Description: max 50 characters
- Amount: positive, rounded to 2 decimal places
- Date: valid format, not in the future

### Error Handling
- **Global Exception Handler Middleware**
- **ProblemDetails (RFC 7807)** format for all errors
- Structured error responses with error codes

### Financial Precision
- All monetary values use `decimal` type
- Rounding only at the final display step
- `MidpointRounding.AwayFromZero` for financial calculations

## ?? Getting Started

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 or VS Code

### Running the Application

```bash
# Clone the repository
git clone <repository-url>
cd WexChallenge

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project WexChallenge/WexChallenge.csproj
```

The API will be available at:
- Swagger UI: https://localhost:5001 (or http://localhost:5000)

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/WexChallenge.UnitTests
dotnet test tests/WexChallenge.IntegrationTests
```

### Docker

```bash
# Build from solution root
docker build -t wexchallenge -f WexChallenge/Dockerfile .

# Run container
docker run -p 8080:8080 wexchallenge
```

## ?? API Endpoints

### Create Purchase
```http
POST /api/purchases
Content-Type: application/json

{
    "description": "Office Supplies",
    "transactionDate": "2024-01-15",
    "amountInUsd": 125.50
}
```

### Get Purchase
```http
GET /api/purchases/{id}
```

### Get All Purchases
```http
GET /api/purchases
```

### Get Purchase with Currency Conversion
```http
GET /api/purchases/{id}/convert?currency=Brazil-Real
```

Response:
```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "description": "Office Supplies",
    "transactionDate": "2024-01-15T00:00:00",
    "originalAmountInUsd": 125.50,
    "targetCurrency": "Real",
    "exchangeRate": 4.95,
    "exchangeRateDate": "2024-01-12T00:00:00",
    "convertedAmount": 621.23
}
```

## ?? Currency Conversion Rules

1. Exchange rate must be **less than or equal to** the purchase date
2. Exchange rate must be within **6 months** of the purchase date
3. If no rate is available, returns HTTP 422 with descriptive error
4. Converted amount is rounded to **2 decimal places**

## ?? Testing Strategy

### Unit Tests
- Domain entities (Purchase, ExchangeRate)
- Validators (CreatePurchaseRequestValidator)
- Application services (PurchaseService)
- Uses **NSubstitute** for mocking

### Integration Tests
- Full API pipeline tests
- Uses **WebApplicationFactory**
- In-memory database for isolation
- Mocked external services

## ?? NuGet Packages

| Package | Purpose |
|---------|---------|
| Microsoft.EntityFrameworkCore.Sqlite | SQLite database provider |
| FluentValidation | Request validation |
| Polly | Resilience policies |
| Swashbuckle.AspNetCore | Swagger/OpenAPI |
| xUnit | Test framework |
| FluentAssertions | Test assertions |
| NSubstitute | Mocking framework |

## ?? License

This project is for evaluation purposes only.
