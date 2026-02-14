# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for restore
COPY server/EmployeeManagementSystem.Domain/EmployeeManagementSystem.Domain.csproj EmployeeManagementSystem.Domain/
COPY server/EmployeeManagementSystem.Application/EmployeeManagementSystem.Application.csproj EmployeeManagementSystem.Application/
COPY server/EmployeeManagementSystem.Infrastructure/EmployeeManagementSystem.Infrastructure.csproj EmployeeManagementSystem.Infrastructure/
COPY server/EmployeeManagementSystem.Api/EmployeeManagementSystem.Api.csproj EmployeeManagementSystem.Api/

# Restore dependencies
RUN dotnet restore EmployeeManagementSystem.Api/EmployeeManagementSystem.Api.csproj

# Copy full source
COPY server/EmployeeManagementSystem.Domain/ EmployeeManagementSystem.Domain/
COPY server/EmployeeManagementSystem.Application/ EmployeeManagementSystem.Application/
COPY server/EmployeeManagementSystem.Infrastructure/ EmployeeManagementSystem.Infrastructure/
COPY server/EmployeeManagementSystem.Api/ EmployeeManagementSystem.Api/

# Publish
RUN dotnet publish EmployeeManagementSystem.Api/EmployeeManagementSystem.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "EmployeeManagementSystem.Api.dll"]
