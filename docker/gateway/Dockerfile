# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy NuGet configuration and gateway project file
COPY gateway/nuget.config ./
COPY gateway/EmployeeManagementSystem.Gateway/EmployeeManagementSystem.Gateway.csproj EmployeeManagementSystem.Gateway/

# Restore dependencies (uses sources from nuget.config)
RUN dotnet restore EmployeeManagementSystem.Gateway/EmployeeManagementSystem.Gateway.csproj

# Copy full source
COPY gateway/EmployeeManagementSystem.Gateway/ EmployeeManagementSystem.Gateway/

# Publish
RUN dotnet publish EmployeeManagementSystem.Gateway/EmployeeManagementSystem.Gateway.csproj \
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

ENTRYPOINT ["dotnet", "EmployeeManagementSystem.Gateway.dll"]
