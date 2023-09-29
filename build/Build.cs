using System.Collections.Generic;
using System.Linq;
using Hexagrams.Extensions.Common.Serialization;
using Hexagrams.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Nuke.Common.Tools.Docker.DockerTasks;

// ReSharper disable RedundantExtendsListEntry
// ReSharper disable InconsistentNaming

[DotNetVerbosityMapping]
[ShutdownDotNetAfterServerBuild]
partial class Build : NukeBuild,
    IHasGitRepository,
    IHasVersioning,
    IRestore,
    IFormat,
    IClean,
    ICompile,
    ITest,
    IReportCoverage
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => ((ICompile) x).Compile);

    protected override void OnBuildInitialized() => Log.Information("{VersionInfo}", GitVersion.ToJson(true));

    [Required]
    [Solution]
    readonly Solution Solution;
    Solution IHasSolution.Solution => Solution;

    [CI]
    readonly GitHubActions GitHubActions;

    [Required]
    [GitRepository]
    readonly GitRepository GitRepository;
    GitRepository IHasGitRepository.GitRepository => GitRepository;

    [Required]
    [GitVersion(NoFetch = true)]
    readonly GitVersion GitVersion;
    GitVersion IHasVersioning.Versioning => GitVersion;

    IEnumerable<AbsolutePath> IFormat.ExcludedFormatPaths => new[]
    {
        AbsolutePath.Create(RootDirectory.GetRelativePathTo(
            Solution.GetAllProjects("Spenses.Resources.Relational").Single().Directory / "Migrations"))
    };

    Target ICompile.Compile => _ => _
        .Inherit<ICompile>()
        .DependsOn<IFormat>(x => x.VerifyFormat);


    Project ApiProject => Solution.GetAllProjects("Spenses.Api").Single();

    string DockerTag => IsServerBuild ? GitVersion.NuGetVersionV2 : "dev";

    string DockerImageName => $"spenses-api:{DockerTag}";

    Target BuildDockerImage => _ => _
        .Description("Build the docker images for the project.")
        .Executes(() =>
        {
            DockerBuild(s => s
                .SetForceRm(true)
                .SetProcessWorkingDirectory(ApiProject.Directory)
                .SetFile(ApiProject.Directory / "Dockerfile")
                .SetTag(new List<string>
                {
                    $"{DockerImageName}:{DockerTag}"
                })
                .SetPath(RootDirectory));
        });
}
