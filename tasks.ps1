function upaspire {
    dotnet run --project (Join-Path $PSScriptRoot 'tools\AppHost\Stagehand.AppHost')
}

function createcluster {
    k3d cluster create stagehand --servers 1 --agents 1 --api-port 127.0.0.1:6445 `
        --port "8000:80@loadbalancer" --port "8443:443@loadbalancer"
}

function startcluster {
    k3d cluster start stagehand
    kubectl config use-context k3d-stagehand
}

function stopcluster {
    k3d cluster stop stagehand
}

function nodes {
    kubectl get nodes -o wide @args
}

function pods {
    kubectl get pods -A @args
}

function clusterimages {
    docker exec k3d-stagehand-server-0 crictl images
}

function applydb {
    foreach ($svc in Get-ChildItem (Join-Path $PSScriptRoot 'deploy') -Directory)
    {
        $dev = Join-Path $svc.FullName 'db\overlays\dev'
        if (Test-Path $dev)
        {
            Write-Host "==> applying $($svc.Name) db"
            kubectl apply -k $dev
        }
    }
}

function applyrabbitmq {
    kubectl apply -k (Join-Path $PSScriptRoot 'deploy\platform\rabbitmq\overlays\dev')
    kubectl rollout status statefulset/rabbitmq -n messaging --timeout=300s
}

function pfrabbitmq {
    # Blocking: management UI at http://localhost:15672 (see the credentials secret).
    kubectl port-forward svc/rabbitmq -n messaging 15672:15672
}

function applykeycloak {
    kubectl apply -k (Join-Path $PSScriptRoot 'deploy\platform\keycloak\overlays\dev')
    kubectl rollout restart deploy/keycloak -n identity
    kubectl rollout status deploy/keycloak -n identity
}

function upk8s {
    # Routine deploy-all onto an existing cluster (operator already installed).
    startcluster
    applydb
    applyrabbitmq
    applykeycloak

    # DBs must be Ready before services run their migration jobs.
    kubectl wait --for=condition=Ready clusters.postgresql.cnpg.io --all -A --timeout=300s

    # Ship every service — auto-discovered (shipcatalog, shipgateway, ...).
    foreach ($ship in Get-Command -CommandType Function -Name 'ship*' -ErrorAction SilentlyContinue)
    {
        Write-Host "==> $($ship.Name)"
        & $ship.Name
    }
}

function bootstrapcluster {
    # One-time, fresh cluster: create -> install CNPG operator -> deploy everything.
    createcluster

    helm repo add cnpg https://cloudnative-pg.github.io/charts
    helm repo update cnpg
    helm install cnpg cnpg/cloudnative-pg -n cnpg-system --create-namespace
    kubectl rollout status deploy/cnpg-cloudnative-pg -n cnpg-system

    upk8s
}

function pfkeycloak {
    # Blocking: admin console at http://localhost:8081 (local 8081 -> keycloak 8080).
    kubectl port-forward svc/keycloak -n identity 8081:8080
}
