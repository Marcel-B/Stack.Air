FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 6000

FROM microsoft/dotnet:2.2-sdk AS build

#FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY Air.csproj Air/
COPY ["NuGet.config", "Air/"]
RUN dotnet restore "Air/Air.csproj" --configfile Air/NuGet.config
COPY . "Air"
WORKDIR "/src/Air"
RUN dotnet build "Air.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Air.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Air.dll"]