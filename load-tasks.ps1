. (Join-Path $PSScriptRoot 'tasks.ps1')

foreach ($area in 'services', 'platform')
{
    $areaPath = Join-Path $PSScriptRoot "src\$area"
    if (-not (Test-Path $areaPath)) { continue }

    foreach ($svc in Get-ChildItem $areaPath -Directory)
    {
        $svcTasks = Join-Path $svc.FullName 'tasks.ps1'
        if (Test-Path $svcTasks) { . $svcTasks }
    }
}