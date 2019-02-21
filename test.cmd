@echo off
.dotnet\dotnet test test\Microsoft.TestTemplates.AcceptanceTests.sln --logger:trx
exit /b %ErrorLevel%
