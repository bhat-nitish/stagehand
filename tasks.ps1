function buildcatalog {
    dotnet build (Join-Path $PSScriptRoot 'src/services/catalog/Stagehand.Catalog.Api')
}

function runcatalog {
    dotnet run --project (Join-Path $PSScriptRoot 'tools/AppHost/Stagehand.AppHost')
}

function dbuildcatalog {
    docker build `
        -f (Join-Path $PSScriptRoot 'src/services/catalog/Stagehand.Catalog.Api/Dockerfile') `
        -t stagehand-catalog:dev `
        $PSScriptRoot
}

function efcatalog {
    dotnet ef @args `
        --project (Join-Path $PSScriptRoot 'src/services/catalog/Stagehand.Catalog.Infrastructure') `
        --startup-project (Join-Path $PSScriptRoot 'src/services/catalog/Stagehand.Catalog.Api')
}

function druncatalog {
    docker network inspect stagehand *> $null
    if ($LASTEXITCODE -ne 0) { docker network create stagehand | Out-Null }

    docker start stagehand-postgres *> $null
    if ($LASTEXITCODE -ne 0) {
        docker run -d --name stagehand-postgres --network stagehand `
            -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=catalog `
            -v stagehand-pgdata:/var/lib/postgresql/data `
            -p 5432:5432 postgres:17 | Out-Null
        Write-Host 'Waiting for Postgres to initialize...'
        Start-Sleep -Seconds 5
    }

    docker run --rm --name catalog --network stagehand `
        -e 'ConnectionStrings__Catalog=Host=stagehand-postgres;Port=5432;Database=catalog;Username=postgres;Password=postgres' `
        -e 'ASPNETCORE_ENVIRONMENT=Development' `
        -p 8080:8080 stagehand-catalog:dev
}

function dstopcatalog {
    docker stop catalog *> $null
    docker stop stagehand-postgres *> $null
}
