param(
    [Parameter(Mandatory = $true)][string] $NugetApiKey,
    [Parameter(Mandatory = $false)][string] $Configuration = "Release",
    [Parameter(Mandatory = $false)][string] $VersionPrefix = "0.1.0",
    [Parameter(Mandatory = $false)][string] $VersionSuffix = "",
    [Parameter(Mandatory = $false)][string] $OutputPath = ""
)

. ./dotnetfunctions.ps1

Write-Host "Building solution..." -ForegroundColor Green
& $dotnet build $solutionPath

$fullVersion = $VersionPrefix + $VersionSuffix

Write-Host "Packing KongVerge version $fullVersion" -ForegroundColor Green
Remove-Item $OutputPath\*.*  -Force -Recurse
& $dotnet pack .\src\Kongverge\KongVerge.csproj -o $OutputPath /p:PackageVersion=$fullVersion

Write-Host "Pushing KongVerge version $fullVersion" -ForegroundColor Green
& $dotnet nuget push $OutputPath\*.nupkg --source http://packages.je-labs.com/nuget/Global/ --api-key $NugetApiKey
