# Bulldog

[![NuGet Version](https://img.shields.io/nuget/v/Bulldog.svg)](https://www.nuget.org/packages/Bulldog)

<img src="./Bulldog.png" width="300px" />

An opinionated base library for building dotnet command line tools

## Prerequisites

* .NET 7.0 SDK
* nuke global tool

## Build the solution.

```dotnet build Bulldog.sln```

This project builds with [Nuke](https://nuke.build/).

To run the build call `build.cmd` which runs the FullBuild target of the nuke build [project](build/Build.csproj) (The same target that is run as part of CI)

### Testing

Tests will be run as part of `FullBuild` target. To run them explicitly use:

```build.cmd Test```

Or unit tests and:

```build.cmd SmokeTest```

For the smoke tests.

## Usage

To use simply add a package reference to Bulldog package. The package defines a ToolBase class to inherit from which which handles the basics of:

- Logging initialisation with Serilog
- Dependency Injetion initialisation
- Cancellation handling
- CommandLine parsing with CommandlineParser library

The smoke test builds and runs an example [tool](tests/TestTool) which can be used as a reference.

## Why

Removes the boiler plate setup when writing command line tooling to allow you to focus on the important logic. 

It is opinionated enough to enforce some standardisation but with some extension points to avoid being too restrictive.

## Versioning

Versioned using NerdBank.GitVersioning

