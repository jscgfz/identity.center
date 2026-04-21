# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Identity.Center.Api/Identity.Center.Api.csproj", "Identity.Center.Api/"]
COPY ["BuildingBlocks.Scheduler/BuildingBlocks.Scheduler.csproj", "BuildingBlocks.Scheduler/"]
COPY ["Identity.Center.Application/Identity.Center.Application.csproj", "Identity.Center.Application/"]
COPY ["Identity.Center.Domain/Identity.Center.Domain.csproj", "Identity.Center.Domain/"]
COPY ["Identity.Center.Infrastructure/Identity.Center.Infrastructure.csproj", "Identity.Center.Infrastructure/"]
COPY ["Identity.Center.Persistence/Identity.Center.Persistence.csproj", "Identity.Center.Persistence/"]
RUN dotnet restore "./Identity.Center.Api/Identity.Center.Api.csproj"
COPY . .
WORKDIR "/src/Identity.Center.Api"
RUN dotnet build "./Identity.Center.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.Center.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Center.Api.dll"]