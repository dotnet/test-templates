function InitializeCustomSDKToolset {

  if (-not $restore) {
    return
  }

  # The following frameworks and tools are used only for testing.
  # Do not attempt to install them in source build.
  if ($env:DotNetBuildFromSource -eq "true") {
    return
  }

  $cli = InitializeDotnetCli -install:$true
  $dotnetRoot = $env:DOTNET_INSTALL_DIR
  $installScript = GetDotNetInstallScript $dotnetRoot

  $versions = @(
    "1.0.5"
    "1.1.2"
    "2.1.0"
    "2.2.1"
    "2.2.2"
    "3.1.0"
    "5.0.1"
    "6.0.1"
    "7.0.100-rc.2.22454.1"
    # version from global.json (8.0.0) will be installed automatically
  )

  foreach ($version in $versions) {
    InstallDotNetSharedFramework $version -DotnetRoot $dotnetRoot -InstallScript $installScript
  }
}

function InstallDotNetSharedFramework([string]$version) {
  $fxDir = Join-Path $dotnetRoot "shared\Microsoft.NETCore.App\$version"

  if (!(Test-Path $fxDir)) {    
    & $installScript -Version $version -InstallDir $dotnetRoot -Runtime "dotnet"

    if($lastExitCode -ne 0) {
      throw "Failed to install shared Framework $version to '$dotnetRoot' (exit code '$lastExitCode')."
    }
  }
}

InitializeCustomSDKToolset
