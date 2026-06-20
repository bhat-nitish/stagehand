function buildgateway {
    dotnet build (Join-Path $PSScriptRoot 'Stagehand.Gateway')
}

function shipgateway {
    # Build -> import into k3d -> helm deploy, all on one immutable timestamp tag
    # (same pattern as shipcatalog: changed tag => natural rollout + rollback-by-tag).
    $repo  = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
    $tag   = "dev-$(Get-Date -Format 'yyyyMMddHHmmss')"
    $image = "stagehand-gateway:$tag"

    docker build `
        -f (Join-Path $PSScriptRoot 'Stagehand.Gateway\Dockerfile') `
        -t $image `
        $repo
    k3d image import $image -c stagehand
    helm upgrade --install gateway (Join-Path $repo 'deploy\platform\gateway\app') `
        --namespace gateway `
        --create-namespace `
        --set image.tag=$tag
}

function pfgateway {
    kubectl port-forward svc/gateway -n gateway 8080:8080
}