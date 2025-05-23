# Overview

This repository contains test templates that are consumed by `dotnet new`. The templates are C#, F# and VB languages; for MSTest, NUnit and XUnit testing frameworks. 
It also contains classic .NET Framework and Universal Test templates for C# and VB. 

# Usage

You can create new test project with `dotnet new mstest`, `dotnet new nunit`, or `dotnet new xunit`. Full documentation for `dotnet new` is [here](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new). And here is documentation for each template: [mstest or xunit](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new-sdk-templates#test), [nunit](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new-sdk-templates#nunit).

# Contributing

This repository consists of:
- `Templates` folder contains "legacy" templates for .NET Framework test projects in VisualStudio.

## Acquire

- Fork this repository.
- Clone the forked repository to your local machine.
  - `main` is our default branch. It should be checked out for you automatically.
  - Create topic branch to hold your change e.g. `git checkout -b "update-mstest"`.
  - Make your changes, commit, and make PR that targets our `main` branch.
  
## Build & Run

- Open up a command prompt and navigation to the root of your source code.
- Run the build script appropriate your environment.
     - **Windows:** [build.cmd](https://github.com/dotnet/test-templates/blob/main/build.cmd)
- When running the build script creates the nuget packages for net core test templates and vsix for classic test templates.
- The build produces the template NuGet packages currently has a dependency on **nuget.exe**. 
- Because of this, those that wish to `install` using the **template NuGet packages** will need to be on Windows in order to produce the appropriate assets. 

## Note

Please note the templates inside the `template_feed` folder were moved into the [dotnet/sdk](https://github.com/dotnet/sdk).