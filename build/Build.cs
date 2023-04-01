using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    [Parameter]
    public AbsolutePath Solution {  get; set; }
    public static int Main () => Execute<Build>(x => x.FullBuild);

    Target FullBuild => _ => _  .DependsOn(Clean, Restore, Test, Compile, SmokeTest);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetCleanSettings settings = new DotNetCleanSettings()
                .SetProject(Solution)
                .SetConfiguration(Configuration);

            DotNetTasks.DotNetClean(settings);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            var settings = new DotNetRestoreSettings()
                .SetProjectFile(Solution);

            DotNetTasks.DotNetRestore(settings);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var settings = new DotNetBuildSettings()
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoRestore(true);

            DotNetTasks.DotNetBuild(settings);
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Log.Information("Probably should have some tests!");

            var settings = new DotNetTestSettings()
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(true);

            //DotNetTasks.DotNetTest(settings);
        });

    Target SmokeTest => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Executes(() =>
        {
            var settings = new DotNetRunSettings()
                .SetProjectFile(Solution.Parent / "tests" / "TestTool" / "TestTool.csproj")
                .SetConfiguration(Configuration)
                .SetFramework("net6.0")
                .SetNoBuild(true)
                .SetApplicationArguments("--verbosity Debug");

            DotNetTasks.DotNetRun(settings);
        });
}
