echo off
dotnet build .\Bulldog.sln && (echo Build successful) || (exit 1)

dotnet test .\Bulldog.sln --no-build && (echo Build successful) || (exit 1)

dotnet run --project .\tests\TestTool\TestTool.csproj -- --verbosity Debug && (echo Validated library works) || (exit 1)