FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Stagehand.slnx ./
COPY Directory.Build.props Directory.Packages.props global.json .editorconfig ./

COPY src/building-blocks/Stagehand.SharedKernel/Stagehand.SharedKernel.csproj src/building-blocks/Stagehand.SharedKernel/
COPY src/building-blocks/Stagehand.SharedKernel.Application/Stagehand.SharedKernel.Application.csproj src/building-blocks/Stagehand.SharedKernel.Application/
COPY src/services/inventory/Stagehand.Inventory.Domain/Stagehand.Inventory.Domain.csproj src/services/inventory/Stagehand.Inventory.Domain/
COPY src/services/inventory/Stagehand.Inventory.Application/Stagehand.Inventory.Application.csproj src/services/inventory/Stagehand.Inventory.Application/
COPY src/services/inventory/Stagehand.Inventory.Infrastructure/Stagehand.Inventory.Infrastructure.csproj src/services/inventory/Stagehand.Inventory.Infrastructure/
COPY src/services/inventory/Stagehand.Inventory.Api/Stagehand.Inventory.Api.csproj src/services/inventory/Stagehand.Inventory.Api/
COPY tools/ServiceDefaults/Stagehand.ServiceDefaults/Stagehand.ServiceDefaults.csproj tools/ServiceDefaults/Stagehand.ServiceDefaults/

RUN dotnet restore src/services/inventory/Stagehand.Inventory.Api/Stagehand.Inventory.Api.csproj

COPY src/ src/
COPY tools/ServiceDefaults/ tools/ServiceDefaults/

RUN dotnet publish src/services/inventory/Stagehand.Inventory.Api/Stagehand.Inventory.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Stagehand.Inventory.Api.dll"]
