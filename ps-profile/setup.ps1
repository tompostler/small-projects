if (-not (Test-Path $PROFILE.CurrentUserAllHosts)) {
    New-Item -ItemType File -Path $PROFILE.CurrentUserAllHosts -Verbose -Force
}
Copy-Item -Path $PSScriptRoot\profile.ps1 -Destination $PROFILE.CurrentUserAllHosts -Verbose -Force