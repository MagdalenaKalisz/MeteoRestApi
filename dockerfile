# Use the .NET SDK image to build and publish the .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory to /app
WORKDIR /app

# Copy the solution and restore any dependencies
COPY SofomoMeteoRestAPI.sln ./

# Copy project files for each library
COPY Library.Application/Library.Application.csproj ./Library.Application/
COPY Library.Buses/Library.Buses.csproj ./Library.Buses/
COPY Library.Buses.Abstractions/Library.Buses.Abstractions.csproj ./Library.Buses.Abstractions/
COPY Library.Domain/Library.Domain.csproj ./Library.Domain/
COPY Library.Extensions/Library.Extensions.csproj ./Library.Extensions/
COPY Library.Extensions.Hosting/Library.Extensions.Hosting.csproj ./Library.Extensions.Hosting/
COPY Library.Identifiers/Library.Identifiers.csproj ./Library.Identifiers/
COPY Library.Persistence/Library.Persistence.csproj ./Library.Persistence/
COPY Meteo.Api/Meteo.Api.csproj ./Meteo.Api/
COPY Meteo.Application/Meteo.Application.csproj ./Meteo.Application/
COPY Meteo.Domain/Meteo.Domain.csproj ./Meteo.Domain/
COPY Meteo.Infrastructure/Meteo.Infrastructure.csproj ./Meteo.Infrastructure/
COPY Meteo.IntegrationTests/Meteo.IntegrationTests.csproj ./Meteo.IntegrationTests/
COPY Meteo.Persistence.Json/Meteo.Persistence.Json.csproj ./Meteo.Persistence.Json/
COPY Meteo.Persistence.PostgreSql/Meteo.Persistence.PostgreSql.csproj ./Meteo.Persistence.PostgreSql/

# Restore dependencies
RUN dotnet restore "SofomoMeteoRestAPI.sln"

# Copy the entire project
COPY . ./

# Build and publish the application
RUN dotnet build "Meteo.Api/Meteo.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Meteo.Api/Meteo.Api.csproj" -c Release --no-restore -o /app/publish

# Use the .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the application files from the build stage
COPY --from=build /app/publish .

# Install curl and PostgreSQL client to check if db is ready
RUN apt update && apt install -y curl postgresql-client

# Copy the entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Use the entrypoint script to start the app and run migrations
ENTRYPOINT ["/entrypoint.sh"]
