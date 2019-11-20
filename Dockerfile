FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
EXPOSE 5063

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

#FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ./Air/Air.csproj Air/
COPY ["./Air/NuGet.config", "Air/"]
RUN dotnet restore "Air/Air.csproj" --configfile nuget.config
COPY "./Air" "Air"
WORKDIR "/src/Air"
RUN dotnet build "Air.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Air.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Air.dll"]