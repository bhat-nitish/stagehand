function createcluster {
    k3d cluster create stagehand --servers 1 --agents 1 --api-port 127.0.0.1:6445
}

function startcluster {
    k3d cluster start stagehand
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
