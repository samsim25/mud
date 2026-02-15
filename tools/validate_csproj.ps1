#!/usr/bin/env pwsh
$errors = @()
Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object {
    try {
        [xml](Get-Content $_.FullName) > $null
        Write-Output "OK: $($_.FullName)"
    } catch {
        Write-Output "ERROR parsing $($_.FullName): $($_.Exception.Message)"
        $errors += $_.FullName
    }
}
if ($errors.Count -gt 0) {
    Write-Output "\nOne or more project files failed XML parse."
    exit 1
} else {
    Write-Output "\nAll project files parsed successfully."
    exit 0
}
