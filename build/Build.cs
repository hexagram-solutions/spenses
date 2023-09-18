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
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.EntityFramework;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.EntityFramework.EntityFrameworkTasks;

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


    bool IReportCoverage.CreateCoverageHtmlReport => true;

    IEnumerable<Project> ITest.TestProjects => Partition.GetCurrent(Solution.GetAllProjects("*.Tests"));

    Target RestoreTools => _ => _
        .Executes(() =>
        {
            DotNetToolRestore(_ => _);
        });

    Target MigrateDatabase => _ => _
        .DependsOn(RestoreTools)
        .Requires(() => SqlServerConnectionString)
        .Executes(() =>
        {
            var dbContextProject = Solution.GetAllProjects("Spenses.Resources.Relational").Single();

            EntityFrameworkDatabaseUpdate(s => s
                .SetProject(dbContextProject)
                .SetConnection(SqlServerConnectionString));
        });

    Target IntegrationTestSetup => _ => _
        .DependsOn(MigrateDatabase)
        .Requires(() => SqlServerConnectionString)
        .Executes(() =>
        {
            var relationalSetupTool = Solution.GetAllProjects("Spenses.Tools.Setup").Single();

            DotNetRun(s => s
                .SetProjectFile(relationalSetupTool)
                .SetApplicationArguments($"seed --connection \"{SqlServerConnectionString}\""));
        });

    Target IntegrationTest => _ => _
        .DependsOn<IRestore>()
        .DependsOn(IntegrationTestSetup)
        .DependsOn()
        .Produces(this.FromComponent<IReportCoverage>().CoverageReportDirectory / "*.trx")
        .Produces(this.FromComponent<IReportCoverage>().CoverageReportDirectory / "*.xml")
        .Executes(() =>
        {
            var integrationTestProjects = Solution.GetAllProjects("*.IntegrationTests");

            DotNetTest(_ => _
                .Apply(this.FromComponent<ITest>().TestSettingsBase)
                .SetVerbosity(DotNetVerbosity.Minimal)
                .CombineWith(integrationTestProjects, (_, v) => _
                    .Apply(this.FromComponent<ITest>().TestProjectSettingsBase, v)),
                completeOnFailure: true);
        });

    Target IReportCoverage.ReportCoverage => _ => _
        .Inherit<IReportCoverage>()
        .TriggeredBy(IntegrationTest)
        .Consumes(IntegrationTest);
}
