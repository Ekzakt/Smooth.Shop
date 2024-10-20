# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Smooth.Shop/src/Smooth.Shop/Smooth.Shop.csproj", "Smooth.Shop/src/Smooth.Shop/"]
COPY ["Smooth.Shop/src/Smooth.Shop.Application/Smooth.Shop.Application.csproj", "Smooth.Shop/src/Smooth.Shop.Application/"]
COPY ["Smooth.Shop/src/Smooth.Shop.FakeData/Smooth.Shop.FakeData.csproj", "Smooth.Shop/src/Smooth.Shop.FakeData/"]
COPY ["Smooth.Shop/src/Smooth.Shop.Infrastructure/Smooth.Shop.Infrastructure.csproj", "Smooth.Shop/src/Smooth.Shop.Infrastructure/"]
RUN dotnet restore "./Smooth.Shop/src/Smooth.Shop/Smooth.Shop.csproj"
COPY . .
WORKDIR "/src/Smooth.Shop/src/Smooth.Shop"
RUN dotnet build "./Smooth.Shop.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Smooth.Shop.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smooth.Shop.dll"]