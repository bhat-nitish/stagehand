function buildreservations {
    dotnet build (Join-Path $PSScriptRoot 'Stagehand.Reservations.Api')
}

function dbuildreservations {
    $repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
    docker build `
        -f (Join-Path $PSScriptRoot 'Stagehand.Reservations.Api\Dockerfile') `
        -t stagehand-reservations:dev `
        $repo
}

function efreservations {
    dotnet ef @args `
        --project (Join-Path $PSScriptRoot 'Stagehand.Reservations.Infrastructure') `
        --startup-project (Join-Path $PSScriptRoot 'Stagehand.Reservations.Api')
}

function drunreservations {
    docker network inspect stagehand *> $null
    if ($LASTEXITCODE -ne 0) { docker network create stagehand | Out-Null }

    docker start stagehand-postgres *> $null
    if ($LASTEXITCODE -ne 0) {
        docker run -d --name stagehand-postgres --network stagehand `
            -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=reservations `
            -v stagehand-pgdata:/var/lib/postgresql/data `
            -p 5432:5432 postgres:17 | Out-Null
        Write-Host 'Waiting for Postgres to initialize...'
        Start-Sleep -Seconds 5
    }

    docker run --rm --name reservations --network stagehand `
        -e 'ConnectionStrings__Reservations=Host=stagehand-postgres;Port=5432;Database=reservations;Username=postgres;Password=postgres' `
        -e 'ASPNETCORE_ENVIRONMENT=Development' `
        -p 8084:8080 stagehand-reservations:dev
}

function dstopreservations {
    docker stop reservations *> $null
    docker stop stagehand-postgres *> $null
}

function reservationsns {
    kubectl config set-context --current --namespace=reservations
}

function pfreservations {
    kubectl port-forward svc/reservations -n reservations 8084:8080
}

function shipreservations {
    # Full k8s deploy with an immutable tag: build -> import -> helm deploy,
    # all sharing one fresh timestamp tag. The changed tag makes helm roll out
    # new pods naturally (no rollout restart), and lets you roll back by tag.
    $repo  = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
    $tag   = "dev-$(Get-Date -Format 'yyyyMMddHHmmss')"
    $image = "stagehand-reservations:$tag"

    docker build `
        -f (Join-Path $PSScriptRoot 'Stagehand.Reservations.Api\Dockerfile') `
        -t $image `
        $repo
    k3d image import $image -c stagehand
    helm upgrade --install reservations (Join-Path $repo 'deploy\reservations\app') `
        --namespace reservations `
        --set image.tag=$tag
}
