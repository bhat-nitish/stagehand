FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Stagehand.slnx ./
COPY Directory.Build.props Directory.Packages.props global.json .editorconfig ./

COPY src/building-blocks/Stagehand.SharedKernel/Stagehand.SharedKernel.csproj src/building-blocks/Stagehand.SharedKernel/
COPY src/building-blocks/Stagehand.SharedKernel.Application/Stagehand.SharedKernel.Application.csproj src/building-blocks/Stagehand.SharedKernel.Application/
COPY src/building-blocks/Stagehand.SharedKernel.Bus/Stagehand.SharedKernel.Bus.csproj src/building-blocks/Stagehand.SharedKernel.Bus/
COPY src/building-blocks/Stagehand.Contracts/Stagehand.Contracts.csproj src/building-blocks/Stagehand.Contracts/
COPY src/services/reservations/Stagehand.Reservations.Domain/Stagehand.Reservations.Domain.csproj src/services/reservations/Stagehand.Reservations.Domain/
COPY src/services/reservations/Stagehand.Reservations.Application/Stagehand.Reservations.Application.csproj src/services/reservations/Stagehand.Reservations.Application/
COPY src/services/reservations/Stagehand.Reservations.Infrastructure/Stagehand.Reservations.Infrastructure.csproj src/services/reservations/Stagehand.Reservations.Infrastructure/
COPY src/services/reservations/Stagehand.Reservations.Api/Stagehand.Reservations.Api.csproj src/services/reservations/Stagehand.Reservations.Api/
COPY tools/ServiceDefaults/Stagehand.ServiceDefaults/Stagehand.ServiceDefaults.csproj tools/ServiceDefaults/Stagehand.ServiceDefaults/

RUN dotnet restore src/services/reservations/Stagehand.Reservations.Api/Stagehand.Reservations.Api.csproj

COPY src/ src/
COPY tools/ServiceDefaults/ tools/ServiceDefaults/

RUN dotnet publish src/services/reservations/Stagehand.Reservations.Api/Stagehand.Reservations.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Stagehand.Reservations.Api.dll"]
