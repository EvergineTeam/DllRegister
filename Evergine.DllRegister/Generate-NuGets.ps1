<#
.SYNOPSIS
	Evergine bindings NuGet Packages generator script, (c) 2022 Evergine Team
.DESCRIPTION
	This script generates NuGet packages for the low-level ImGui bindings used in Evergine
	It's meant to have the same behavior when executed locally as when it's executed in a CI pipeline.
.EXAMPLE
	<script> -$versionSuffix -local
.LINK
	https://evergine.com/
#>

param (
	[string]$csprojPath = "Evergine.DllRegister\Evergine.DllRegister.csproj",
	[string]$outputFolderBase = "nupkgs",
	[string]$buildVerbosity = "normal",
	[string]$buildConfiguration = "Release",
    [string]$versionSuffix = ""
)


$ErrorActionPreference = "Stop"
. "$PSScriptRoot\ps_support.ps1"

# Set working directory
$currentDir = (Get-Location).Path
Push-Location $currentDir
Set-Location $PSScriptRoot

# calculate version
$versionWithSuffix = "$(Get-Date -Format "yyyy.M.d").$([string]([int]$(Get-Date -Format "HH")*60+[int]$(Get-Date -Format "mm")))$versionSuffix"

# Predefined variables
$toolsPath = Resolve-Path "Builds\"
$env:Path = "$toolsPath;" + $env:Path
$outputFolder = Join-Path $outputFolderBase $versionWithSuffix
$tempSolutionName = "tempsolutionNuget"

# Utility functions
function LogDebug($line) { Write-Host "##[debug] $line" -Foreground Blue -Background Black }

function Using-Culture([System.Globalization.CultureInfo]$culture, [ScriptBlock]$script)
{
    $OldCulture = [System.Threading.Thread]::CurrentThread.CurrentCulture
    trap
    {
    [System.Threading.Thread]::CurrentThread.CurrentCulture = $OldCulture
    }
    [System.Threading.Thread]::CurrentThread.CurrentCulture = $culture
    $ExecutionContext.InvokeCommand.InvokeScript($script)
    [System.Threading.Thread]::CurrentThread.CurrentCulture = $OldCulture
}

# Utility functions
function LogDebug($line)
{ Write-Host "##[debug] $line" -Foreground Blue -Background Black
}

# Show variables
LogDebug "############## VARIABLES ##############"
LogDebug "Version.............: $versionWithSuffix"
LogDebug "Build configuration.: $buildConfiguration"
LogDebug "Build verbosity.....: $buildVerbosity"
LogDebug "Output folder.......: $outputFolderBase"
LogDebug "#######################################"

# Create output folder
$outputFolder = Join-Path $outputFolderBase $versionWithSuffix
New-Item -ItemType Directory -Force -Path $outputFolder
$absoluteOutputFolder = Resolve-Path $outputFolder

# Locate build tools
$msbuildPath = vswhere -prerelease -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
if(-Not $?) { exit $lastexitcode }
$msbuildPath = Resolve-Path (Join-Path $msbuildPath 'MSBuild\*\Bin\MSBuild.exe')
if (-Not (Test-Path $msbuildPath)) { throw "MSBuild not found." }

$symbols = "false"
#if($buildConfiguration -eq "Debug")
#{
#	$symbols = "true"
#}

# Generate packages
LogDebug "START packaging process"
& $msbuildPath "Evergine.DllRegister\Evergine.DllRegister.csproj" /t:restore,pack /p:Configuration=$buildConfiguration /v:$buildVerbosity /p:PackageOutputPath="$absoluteOutputFolder" /p:IncludeSymbols=$symbols /p:SymbolPackageFormat=snupkg /p:Version=$versionWithSuffix
if($?)
{
   LogDebug "END packaging process"
}
else
{
	LogDebug "ERROR; packaging failed"
   	exit -1
}
