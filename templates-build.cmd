@echo off

SET DN3BASEDIR=%~dp0

SET DN3B=Release
echo Using build configuration "%DN3B%"...

SET DN3FFB=$true

echo "Calling build.ps1"
powershell -NoProfile -NoLogo -Command "& \"%~dp0build.ps1\" -Configuration %DN3B% -CIBuild $true -TemplatesBuild $true -PerformFullFrameworkBuild %DN3FFB%; exit $LastExitCode;"

echo Done.
