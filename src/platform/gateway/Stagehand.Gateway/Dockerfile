FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Directory.Build.props Directory.Packages.props global.json .editorconfig ./

COPY src/platform/gateway/Stagehand.Gateway/Stagehand.Gateway.csproj src/platform/gateway/Stagehand.Gateway/
COPY tools/ServiceDefaults/Stagehand.ServiceDefaults/Stagehand.ServiceDefaults.csproj tools/ServiceDefaults/Stagehand.ServiceDefaults/

RUN dotnet restore src/platform/gateway/Stagehand.Gateway/Stagehand.Gateway.csproj

COPY src/ src/
COPY tools/ServiceDefaults/ tools/ServiceDefaults/

RUN dotnet publish src/platform/gateway/Stagehand.Gateway/Stagehand.Gateway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Stagehand.Gateway.dll"]
