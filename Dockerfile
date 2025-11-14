# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ProjectBudgetManagement.sln ./
COPY src/ProjectBudgetManagement.Api/ProjectBudgetManagement.Api.csproj ./src/ProjectBudgetManagement.Api/
COPY src/ProjectBudgetManagement.Application/ProjectBudgetManagement.Application.csproj ./src/ProjectBudgetManagement.Application/
COPY src/ProjectBudgetManagement.Domain/ProjectBudgetManagement.Domain.csproj ./src/ProjectBudgetManagement.Domain/
COPY src/ProjectBudgetManagement.Infrastructure/ProjectBudgetManagement.Infrastructure.csproj ./src/ProjectBudgetManagement.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/src/ProjectBudgetManagement.Api
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "ProjectBudgetManagement.Api.dll"]
