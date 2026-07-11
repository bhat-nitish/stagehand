function buildinventory {
    dotnet build (Join-Path $PSScriptRoot 'Stagehand.Inventory.Api')
}

function dbuildinventory {
    $repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
    docker build `
        -f (Join-Path $PSScriptRoot 'Stagehand.Inventory.Api\Dockerfile') `
        -t stagehand-inventory:dev `
        $repo
}

function efinventory {
    dotnet ef @args `
        --project (Join-Path $PSScriptRoot 'Stagehand.Inventory.Infrastructure') `
        --startup-project (Join-Path $PSScriptRoot 'Stagehand.Inventory.Api')
}

function druninventory {
    docker network inspect stagehand *> $null
    if ($LASTEXITCODE -ne 0) { docker network create stagehand | Out-Null }

    docker start stagehand-postgres *> $null
    if ($LASTEXITCODE -ne 0) {
        docker run -d --name stagehand-postgres --network stagehand `
            -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=inventory `
            -v stagehand-pgdata:/var/lib/postgresql/data `
            -p 5432:5432 postgres:17 | Out-Null
        Write-Host 'Waiting for Postgres to initialize...'
        Start-Sleep -Seconds 5
    }

    docker run --rm --name inventory --network stagehand `
        -e 'ConnectionStrings__Inventory=Host=stagehand-postgres;Port=5432;Database=inventory;Username=postgres;Password=postgres' `
        -e 'ASPNETCORE_ENVIRONMENT=Development' `
        -p 8082:8080 stagehand-inventory:dev
}

function dstopinventory {
    docker stop inventory *> $null
    docker stop stagehand-postgres *> $null
}

function inventoryns {
    kubectl config set-context --current --namespace=inventory
}

function pfinventory {
    kubectl port-forward svc/inventory -n inventory 8082:8080
}

function shipinventory {
    # Full k8s deploy with an immutable tag: build -> import -> helm deploy,
    # all sharing one fresh timestamp tag. The changed tag makes helm roll out
    # new pods naturally (no rollout restart), and lets you roll back by tag.
    $repo  = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
    $tag   = "dev-$(Get-Date -Format 'yyyyMMddHHmmss')"
    $image = "stagehand-inventory:$tag"

    docker build `
        -f (Join-Path $PSScriptRoot 'Stagehand.Inventory.Api\Dockerfile') `
        -t $image `
        $repo
    k3d image import $image -c stagehand
    helm upgrade --install inventory (Join-Path $repo 'deploy\inventory\app') `
        --namespace inventory `
        --set image.tag=$tag
}
