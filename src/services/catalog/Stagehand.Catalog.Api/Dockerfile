FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Stagehand.slnx ./
COPY Directory.Build.props Directory.Packages.props global.json .editorconfig ./

COPY src/building-blocks/Stagehand.SharedKernel/Stagehand.SharedKernel.csproj src/building-blocks/Stagehand.SharedKernel/
COPY src/building-blocks/Stagehand.SharedKernel.Application/Stagehand.SharedKernel.Application.csproj src/building-blocks/Stagehand.SharedKernel.Application/
COPY src/services/catalog/Stagehand.Catalog.Domain/Stagehand.Catalog.Domain.csproj src/services/catalog/Stagehand.Catalog.Domain/
COPY src/services/catalog/Stagehand.Catalog.Application/Stagehand.Catalog.Application.csproj src/services/catalog/Stagehand.Catalog.Application/
COPY src/services/catalog/Stagehand.Catalog.Infrastructure/Stagehand.Catalog.Infrastructure.csproj src/services/catalog/Stagehand.Catalog.Infrastructure/
COPY src/services/catalog/Stagehand.Catalog.Api/Stagehand.Catalog.Api.csproj src/services/catalog/Stagehand.Catalog.Api/
COPY tools/ServiceDefaults/Stagehand.ServiceDefaults/Stagehand.ServiceDefaults.csproj tools/ServiceDefaults/Stagehand.ServiceDefaults/

RUN dotnet restore src/services/catalog/Stagehand.Catalog.Api/Stagehand.Catalog.Api.csproj

COPY src/ src/
COPY tools/ServiceDefaults/ tools/ServiceDefaults/

RUN dotnet publish src/services/catalog/Stagehand.Catalog.Api/Stagehand.Catalog.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Stagehand.Catalog.Api.dll"]
