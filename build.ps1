#
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

param(
    [string]$Configuration="Debug",
    
    [bool]$CIBuild=$false,

    [switch]$Help)

if($Help)
{
    Write-Host "Usage: .\build.ps1 [-Configuration <CONFIGURATION>] [-Help]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Configuration <CONFIGURATION>                Build the specified Configuration (Debug or Release, default: Debug)"
    Write-Host "  -PerformFullFrameworkBuild <$true|$false>     Whether or not to build for NET45 as well as .NET Core"
    Write-Host "  -Help                                         Display this help message"
    exit 0
}

if (!$PerformFullFrameworkBuild)
{
    Write-Host "Skipping NET45 build..."
}
else
{
    Write-Host "NET45 build is enabled, if invoking from setup, set the environment variable DN3FFB to $false to prevent this..."
}

$RepoRoot = "$PSScriptRoot"
$ArtifactsDir = "$RepoRoot\artifacts"
$env:NUGET_PACKAGES = "$RepoRoot\packages"
$env:CONFIGURATION = $Configuration;
$env:VSWHERE_VERSION = "2.0.2"
$env:MSBUILD_VERSION = "15.0"
$env:NUGET_EXE_Version = "3.4.3"

function Locate-MSBuildPath 
{
    $vsInstallPath = Locate-VsInstallPath

    if([string]::IsNullOrEmpty($vsInstallPath))
    {
        return $null
    }

    $vsInstallPath = Resolve-Path -path $vsInstallPath
    $msbuildPath = Join-Path -path $vsInstallPath -childPath "MSBuild\$env:MSBUILD_VERSION\Bin\msbuild.exe"

    Write-Verbose "msbuildPath is : $msbuildPath"
    return $msbuildPath
}

function Locate-VsInstallPath
{
   $vswhere = Join-Path -path $env:NUGET_PACKAGES -ChildPath "vswhere\$env:VSWHERE_VERSION\tools\vswhere.exe"
   if (!(Test-Path -path $vswhere)) {
       throw "Unable to locate vswhere in path '$vswhere'."
   }

   Write-Verbose "Using '$vswhere' to locate VS installation path."

   $requiredPackageIds = @("Microsoft.Component.MSBuild", "Microsoft.Net.Component.4.6.TargetingPack", "Microsoft.VisualStudio.Component.VSSDK")
   Write-Verbose "VSInstallation requirements : $requiredPackageIds"

   Try
   {
       Write-Verbose "VSWhere command line: $vswhere -latest -prerelease -products * -requires $requiredPackageIds -property installationPath"
       if ($CIBuild) {
           $vsInstallPath = & $vswhere -latest -products * -requires $requiredPackageIds -property installationPath
       }
       else {
           # Allow using pre release versions of VS for dev builds
           $vsInstallPath = & $vswhere -latest -prerelease -products * -requires $requiredPackageIds -property installationPath
       }
   }
   Catch [System.Management.Automation.MethodInvocationException]
   {
       Write-Verbose "Failed to find VS installation with requirements : $requiredPackageIds"
   }

   Write-Verbose "VSInstallPath is : $vsInstallPath"
   return $vsInstallPath
}

function Download-Dotnet {

    # Use a repo-local install directory (but not the artifacts directory because that gets cleaned a lot
    if (!$env:DOTNET_INSTALL_DIR)
    {
        $env:DOTNET_INSTALL_DIR="$RepoRoot\.dotnet\"
    }

    if (!(Test-Path $env:DOTNET_INSTALL_DIR))
    {
        mkdir $env:DOTNET_INSTALL_DIR | Out-Null
    }

    if (Test-Path "$RepoRoot\artifacts")
    {
        rm "$RepoRoot\artifacts" -Force -Recurse
    }

    mkdir "$RepoRoot\artifacts" | Out-Null

    $dotnetInstallScript = "$RepoRoot\artifacts\dotnet-install.ps1"
    $DOTNET_INSTALL_SCRIPT_URL="https://raw.githubusercontent.com/dotnet/cli/master/scripts/obtain/dotnet-install.ps1"
    Invoke-WebRequest $DOTNET_INSTALL_SCRIPT_URL -OutFile $dotnetInstallScript

    & "$RepoRoot\artifacts\dotnet-install.ps1" -Verbose -Version latest
    if($LASTEXITCODE -ne 0) { throw "Failed to install dotnet cli" }

    # Pull in additional shared frameworks.
    # Get netcoreapp1.0 shared components.
    if (!(Test-Path "$env:DOTNET_INSTALL_DIR\shared\Microsoft.NETCore.App\1.0.5")) {
        & $dotnetInstallScript -InstallDir $env:DOTNET_INSTALL_DIR -SharedRuntime -Version '1.0.5' -Channel 'preview'
    }
    
    # Get netcoreapp1.1 shared components.
    if (!(Test-Path "$env:DOTNET_INSTALL_DIR\shared\Microsoft.NETCore.App\1.1.2")) {
        & $dotnetInstallScript -InstallDir $env:DOTNET_INSTALL_DIR -SharedRuntime -Version '1.1.2' -Channel 'release/1.1.0'
    }

    # Get netcoreapp2.0 shared components.
    if (!(Test-Path "$env:DOTNET_INSTALL_DIR\shared\Microsoft.NETCore.App\2.0.0")) {
        & $dotnetInstallScript -InstallDir $env:DOTNET_INSTALL_DIR -SharedRuntime -Version '2.0.0' -Channel 'release/2.0.0'
    }

    # Put the stage0 on the path
    $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
}

function Perform-Restore {

# Restore all the required packages
& dotnet restore $RepoRoot\external.csproj --packages $env:NUGET_PACKAGES -v:minimal -warnaserror

$nugetConfig = Join-Path $RepoRoot Nuget.config
# Call nuget pack on these components.
    $nugetExe = Join-Path $env:NUGET_PACKAGES -ChildPath "Nuget.CommandLine" | Join-Path -ChildPath $env:NUGET_EXE_Version | Join-Path -ChildPath "tools\NuGet.exe"

  Write-Verbose "Starting solution restore..."
  foreach($solutionPath in $TT_Solutions)
  
  {
	Write-Verbose "$nuget restore -verbosity quiet -nonInteractive -configFile $nugetConfig $solutionPath"
	&  $nugetExe restore -verbosity quiet -nonInteractive -configFile $nugetConfig $solutionPath
  }
}

$TT_Solutions = @("Templates\MSTestTemplates.sln","WizardExtensions\WizardExtensions.sln")
$TT_VSmanprojs =@("src\setup\Microsoft.VisualStudio.Templates.CS.MSTestv2.Desktop.UnitTest.vsmanproj",
                   "src\setup\Microsoft.VisualStudio.Templates.CS.MSTestv2.UWP.UnitTest.vsmanproj", 
				   "src\setup\Microsoft.VisualStudio.Templates.VB.MSTestv2.Desktop.UnitTest.vsmanproj",
				   "src\setup\Microsoft.VisualStudio.Templates.VB.MSTestv2.UWP.UnitTest.vsmanproj",
                   "src\setup\Microsoft.VisualStudio.TestTools.MSTestV2.WizardExtension.IntelliTest.vsmanproj", 
                   "src\setup\Microsoft.VisualStudio.TestTools.MSTestV2.WizardExtension.UnitTest.vsmanproj",
                   "src\setup\Microsoft.VisualStudio.Templates.CS.EdgeDriverTestCore.vsmanproj",
                   "src\Setup\Microsoft.VisualStudio.Templates.CS.EdgeDriverTest.vsmanproj")

Download-Dotnet
Perform-Restore
$vsInstallPath = Locate-VsInstallPath
$env:MSBuildDirectory =  Join-Path $vsInstallPath "msbuild"
$env:MSBuildExePath = Locate-MSBuildPath 

    if (-not $env:BUILD_NUMBER)
    {
      $env:BUILD_NUMBER = 0
    }

    if (-not $env:PACKAGE_VERSION)
    {
      $env:PACKAGE_VERSION = "1.0.0"
    }

    $NoTimestampPackageVersion=$env:PACKAGE_VERSION

    if (-not $env:BUILD_QUALITY)
    {
      $env:BUILD_QUALITY = "beta1"
    }

    $NoTimestampPackageVersion=$env:PACKAGE_VERSION + "-" + $env:BUILD_QUALITY

    $TimestampPackageVersion=$NoTimestampPackageVersion + "-" + [System.DateTime]::Now.ToString("yyyyMMdd") + "-" + $env:BUILD_NUMBER


function Build-Templates 
{
    if ((Test-Path $env:MSBuildExePath))
    {
      & $env:MsBuildExePath $RepoRoot\build.proj /p:Configuration=$Configuration
    }
    else
    {
      & dotnet msbuild $RepoRoot\build.proj /p:Configuration=$Configuration
    }

    #& dotnet msbuild $RepoRoot\build.proj /t:Test /p:Configuration=$Configuration
}

function Build-vsmanprojs
{
    if($CIBuild)
    {
        if ((Test-Path $env:MSBuildExePath))
        {
            $msbuild = $env:MSBuildExePath
        }
        else
        {
            $msbuild = "dotnet msbuild"
        }

        foreach($vsmanprojPath in $TT_VSmanprojs)
        {	
	        Write-Verbose "$msbuild /t:Build /p:Configuration=$configuration /tv:$env:MSBUILD_VERSION /m /p:TargetExt=.vsman $vsmanprojPath"
	        & $msbuild /t:Build /p:Configuration=$configuration /tv:$env:MSBUILD_VERSION /m /p:TargetExt=.vsman $vsmanprojPath
	
	        if ($lastExitCode -ne 0) {
		        throw "VSManProj build failed with an exit code of '$lastExitCode'."
	        }
        }
    }
}

Build-Templates
Build-vsmanprojs
