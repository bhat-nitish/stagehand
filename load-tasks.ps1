. (Join-Path $PSScriptRoot 'tasks.ps1')

foreach ($svc in Get-ChildItem (Join-Path $PSScriptRoot 'src\services') -Directory)
{
    $svcTasks = Join-Path $svc.FullName 'tasks.ps1'
    if (Test-Path $svcTasks) { . $svcTasks }
}
