#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/SanedStatrerKit/SanedStatrerKit.csproj", "Services/SanedStatrerKit/"]
COPY ["BuildingBlocks/BuildingBlocks.ServiceContracts/BuildingBlocks.ServiceContracts.csproj", "BuildingBlocks/BuildingBlocks.ServiceContracts/"]
RUN dotnet restore "Services/SanedStatrerKit/SanedStatrerKit.csproj"
COPY . .
WORKDIR "/src/Services/SanedStatrerKit"
RUN dotnet build "SanedStatrerKit.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SanedStatrerKit.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SanedStatrerKit.dll"]