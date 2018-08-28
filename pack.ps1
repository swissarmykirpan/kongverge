param(
    [Parameter(Mandatory = $true)][string] $NugetApiKey,
    [Parameter(Mandatory = $false)][string] $Configuration = "Release",
    [Parameter(Mandatory = $false)][string] $VersionPrefix = "0.1.0",
    [Parameter(Mandatory = $false)][string] $VersionSuffix = "",
    [Parameter(Mandatory = $false)][string] $OutputPath = ""
)

. ./dotnetfunctions.ps1

Remove-Item $OutputPath\*.*  -Force -Recurse

Write-Host "Building solution..." -ForegroundColor Green
& $dotnet build $solutionPath

$fullVersion = $VersionPrefix + $VersionSuffix

Write-Host "Packing Kongverge version $fullVersion" -ForegroundColor Green
& $dotnet pack .\src\Kongverge\Kongverge.csproj -o $OutputPath /p:PackageVersion=$fullVersion

$feedUrl = "http://packages.je-labs.com/nuget/Global/"
Write-Host "Pushing Kongverge version $fullVersion to $feedUrl" -ForegroundColor Green
& $dotnet nuget push $OutputPath\*.nupkg --source $feedUrl --api-key $NugetApiKey
