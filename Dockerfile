# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY TaskManager.Domain/TaskManager.Domain.csproj TaskManager.Domain/
COPY TaskManager.Application/TaskManager.Application.csproj TaskManager.Application/
COPY TaskManager.Infrastructure/TaskManager.Infrastructure.csproj TaskManager.Infrastructure/
COPY TaskManager.API/TaskManager.API.csproj TaskManager.API/

# Restore dependencies for API project (will restore all dependencies)
RUN dotnet restore TaskManager.API/TaskManager.API.csproj

# Copy all source files
COPY . .

# Build the application
WORKDIR /src/TaskManager.API
RUN dotnet build TaskManager.API.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish TaskManager.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
