FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Stack.Air.Server/Stack.Air.Server.csproj", "Stack.Air.Server/"]
RUN dotnet restore "Stack.Air.Server/Stack.Air.Server.csproj"
COPY . .
WORKDIR "/src/Stack.Air.Server"
RUN dotnet build "Stack.Air.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stack.Air.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stack.Air.Server.dll"]