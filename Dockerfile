FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

EXPOSE 6000

# Copy csproj and restore as distinct layers
COPY *.csproj ./
COPY NuGet.config ./
RUN dotnet restore --configfile NuGet.config

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Air.dll"]
