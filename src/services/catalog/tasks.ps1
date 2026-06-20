function buildcatalog {
    dotnet build (Join-Path $PSScriptRoot 'Stagehand.Catalog.Api')
}

function dbuildcatalog {
    $repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
    docker build `
        -f (Join-Path $PSScriptRoot 'Stagehand.Catalog.Api\Dockerfile') `
        -t stagehand-catalog:dev `
        $repo
}

function efcatalog {
    dotnet ef @args `
        --project (Join-Path $PSScriptRoot 'Stagehand.Catalog.Infrastructure') `
        --startup-project (Join-Path $PSScriptRoot 'Stagehand.Catalog.Api')
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

function catalogns {
    kubectl config set-context --current --namespace=catalog
}

function pfcatalog {
    kubectl port-forward svc/catalog -n catalog 8080:8080
}

function shipcatalog {
    # Full k8s deploy with an immutable tag: build -> import -> helm deploy,
    # all sharing one fresh timestamp tag. The changed tag makes helm roll out
    # new pods naturally (no rollout restart), and lets you roll back by tag.
    $repo  = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
    $tag   = "dev-$(Get-Date -Format 'yyyyMMddHHmmss')"
    $image = "stagehand-catalog:$tag"

    docker build `
        -f (Join-Path $PSScriptRoot 'Stagehand.Catalog.Api\Dockerfile') `
        -t $image `
        $repo
    k3d image import $image -c stagehand
    helm upgrade --install catalog (Join-Path $repo 'deploy\catalog\app') `
        --namespace catalog `
        --set image.tag=$tag
}
