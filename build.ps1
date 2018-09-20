param(
    [Parameter(Mandatory = $false)][string] $Configuration = "Release",
    [Parameter(Mandatory = $false)][string] $VersionPrefix = "1.0.0",
    [Parameter(Mandatory = $false)][string] $VersionSuffix = "",
    [Parameter(Mandatory = $false)][string] $OutputPath = ""
)

. ./dotnetfunctions.ps1

Write-Host "Building solution..." -ForegroundColor Green
& $dotnet build $solutionPath

Write-Host "Testing solution..." -ForegroundColor Green
DotNetTest "src\Kongverge.Common.Tests\Kongverge.Common.Tests.csproj"

Write-Host "Publishing solution..." -ForegroundColor Green
DotNetPublish "src\KongVerge\KongVerge.csproj"
