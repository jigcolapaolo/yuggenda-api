# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project files first to improve Docker layer caching
COPY ["src/Yuggenda.Api/Yuggenda.Api.csproj", "src/Yuggenda.Api/"]
COPY ["src/Yuggenda.Application/Yuggenda.Application.csproj", "src/Yuggenda.Application/"]
COPY ["src/Yuggenda.Domain/Yuggenda.Domain.csproj", "src/Yuggenda.Domain/"]
COPY ["src/Yuggenda.Infrastructure/Yuggenda.Infrastructure.csproj", "src/Yuggenda.Infrastructure/"]

RUN dotnet restore "src/Yuggenda.Api/Yuggenda.Api.csproj"

# Copy the remaining source code
COPY . .

RUN dotnet publish "src/Yuggenda.Api/Yuggenda.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Create a non-root user
RUN useradd --create-home --uid 10001 appuser

COPY --from=build /app/publish .

USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "Yuggenda.Api.dll"]