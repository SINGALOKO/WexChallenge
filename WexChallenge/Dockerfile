# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["WexChallenge.sln", "./"]
COPY ["WexChallenge/WexChallenge.csproj", "WexChallenge/"]
COPY ["src/WexChallenge.Domain/WexChallenge.Domain.csproj", "src/WexChallenge.Domain/"]
COPY ["src/WexChallenge.Application/WexChallenge.Application.csproj", "src/WexChallenge.Application/"]
COPY ["src/WexChallenge.Infrastructure/WexChallenge.Infrastructure.csproj", "src/WexChallenge.Infrastructure/"]
COPY ["tests/WexChallenge.UnitTests/WexChallenge.UnitTests.csproj", "tests/WexChallenge.UnitTests/"]
COPY ["tests/WexChallenge.IntegrationTests/WexChallenge.IntegrationTests.csproj", "tests/WexChallenge.IntegrationTests/"]

# Restore dependencies
RUN dotnet restore "WexChallenge/WexChallenge.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/WexChallenge"
RUN dotnet build "WexChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WexChallenge.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WexChallenge.dll"]