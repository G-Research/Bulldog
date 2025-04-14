# Bulldog

[![NuGet Version](https://img.shields.io/nuget/v/Bulldog.svg)](https://www.nuget.org/packages/Bulldog)

<img src="./Bulldog.png" width="300px" />

## Overview

Bulldog is an opinionated base library for building .NET command line tools. It provides a robust foundation that handles common concerns in CLI tool development, allowing developers to focus on implementing their core business logic rather than boilerplate setup.

## Features

- **Logging Integration**: Built-in Serilog configuration for structured logging
- **Dependency Injection**: Automatic DI container setup and configuration
- **Cancellation Handling**: Graceful handling of cancellation tokens and signals
- **Command Line Parsing**: Integration with CommandLineParser library for robust argument handling
- **Standardized Structure**: Enforces best practices while maintaining flexibility
- **Extensible Design**: Provides extension points for customization

## Prerequisites

* .NET 7.0 SDK
* nuke global tool

## Installation

Add the Bulldog package to your project:

```bash
dotnet add package Bulldog
```

## Quick Start

1. Create a new .NET project
2. Add the Bulldog package
3. Inherit from `ToolBase` in your main program class
4. Implement your tool's logic

Example:

```csharp
public class MyTool : ToolBase
{
    protected override async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        // Your tool's logic here
        return 0;
    }
}
```

## Building from Source

This project builds with [Nuke](https://nuke.build/).

To build the solution:
```bash
dotnet build Bulldog.sln
```

To run the build:
```bash
build.cmd
```
This runs the FullBuild target of the nuke build [project](build/Build.csproj) (The same target that is run as part of CI)

### Testing

Tests will be run as part of `FullBuild` target. To run them explicitly:

```bash
build.cmd Test    # For unit tests
build.cmd SmokeTest    # For smoke tests
```

The smoke test builds and runs an example [tool](tests/TestTool) which can be used as a reference.

## Contributing

We welcome contributions! Take a look at the [issues](https://github.com/G-Research/Bulldog/issues) to find something to work on. Please read the [CONTRIBUTING.md](CONTRIBUTING.md) before contributing.

### Development Guidelines
- Follow the existing code style and conventions
- Add tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PRs

## Channels

Community engagement is welcome! Please use the following channels:
- GitHub Issues: For bug reports and feature requests
- Pull Requests: For code changes and reviews
- Discussions: For general questions and community engagement


## Security

Please see our [security policy](SECURITY.md) for details on reporting security vulnerabilities.

## License

This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details.

