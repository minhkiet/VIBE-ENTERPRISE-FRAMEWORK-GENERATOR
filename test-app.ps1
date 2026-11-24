$publishDir = Join-Path $PSScriptRoot 'vibe-setup-gui-wpf\bin\Release\net8.0-windows\win-x64\publish'
$exePath = Join-Path $publishDir 'VibeCodeEnterpriseFramework.exe'
if (-not (Test-Path -LiteralPath $exePath)) {
    throw "Published app not found: $exePath"
}
$p = Start-Process $exePath -PassThru
Start-Sleep 5
if ($p.HasExited) { 
    Write-Host "Crashed: $($p.ExitCode)"
} else { 
    Write-Host "App running OK"
    Stop-Process $p -Force 
}
