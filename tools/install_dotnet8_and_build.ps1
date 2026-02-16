Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile 'dotnet-install.ps1'
& .\dotnet-install.ps1 -Channel 8.0 -Version 8.0.100 -InstallDir "$env:USERPROFILE\.dotnet"
$dotnetPath = Join-Path $env:USERPROFILE '.dotnet\dotnet.exe'
& $dotnetPath --info
& $dotnetPath --list-sdks
& $dotnetPath build 'c:\Users\ROG\Mud\AvalonMudClient\AvalonMudClient.sln'
