echo off

dotnet tool restore

dotnet msbuild /t:PublishArtefact .\src\Bulldog\Bulldog.csproj && (echo Bulldog Publish successful) || (exit 1)