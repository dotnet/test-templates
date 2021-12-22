# Overview

This repository is the home for the .NET Core Test Templates. It also contains Classic .Net Framework and Universal Test templates for C# and VB. 

# Creating new projects

You can create new projects with `dotnet new`, this section will briefly describe that. For more info take a look at
[Announcing .NET Core Tools Updates in VS 2017 RC](https://blogs.msdn.microsoft.com/dotnet/2017/02/07/announcing-net-core-tools-updates-in-vs-2017-rc/).

Let's create a new test project named "NewMSTestProject" in the "src/MyTest" directory. 

```bash
$ dotnet new mstest -n NewMSTestProject 
The template "MSTest Test Project" was created successfully.
```

The project was successfully created on disk as expected in `src/MyTest`. From here, we can run normal `dotnet` commands like `dotnet restore` and `dotnet build`.

We have a pretty good help system built in, including template specific help (_for example `dotnet new mstest --help`_). If you're not sure the syntax please try that,
if you have any difficulties please file a new issue.

# How to build, run & debug the latest

If you're authoring templates, or interested in contributing to this repo, then you're likely interested in how to use the latest version of this experience.
The steps required are outlined below.

## Acquire

- Fork this repository.
- Clone the forked repository to your local machine.
  - **main** is a build branch and does not accept contributions directly.
  - The default branch is the active development branch that accepts contributions and flows to main to produce packages.

## Build & Run

- Open up a command prompt and navigation to the root of your source code.
- Run the build script appropriate your environment.
     - **Windows:** [build.cmd](https://github.com/dotnet/test-templates/blob/main/build.cmd)
- When running the build script creates the nuget packages for net core test templates and vsix for classic test templates.
- The build produces the template NuGet packages currently has a dependency on **nuget.exe**. 
- Because of this, those that wish to `install` using the **template NuGet packages** will need to be on Windows in order to produce the appropriate assets. 

## Test

- Users can test the dotnet core templates that reside in the templates_feed folder by running the test script [test.cmd](https://github.com/dotnet/test-templates/blob/main/test.cmd)
- Note: Please make sure you have run the build script before you run the test script.
