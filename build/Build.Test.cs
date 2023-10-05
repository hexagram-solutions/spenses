using System.Collections.Generic;
using System.Linq;
using Hexagrams.Nuke.Components;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.EntityFramework;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.EntityFramework.EntityFrameworkTasks;


partial class Build
{
    bool IReportCoverage.CreateCoverageHtmlReport => true;

    IEnumerable<Project> ITest.TestProjects => Partition.GetCurrent(Solution.GetAllProjects("*.Tests"));

    public Configure<DotNetTestSettings> TestSettings => _ => _
        .SetExcludeByFile("\\\"*.Generated.cs,**/Resources/Relational/**.cs\\\"");

    Target RestoreTools => _ => _
        .Executes(() =>
        {
            DotNetToolRestore(_ => _);
        });

    Project RelationalSetupTool => Solution.GetAllProjects("Spenses.Tools.Setup").Single();

    Target MigrateDatabase => _ => _
        .Description("Migrate the SQL Server database to the latest version.")
        .DependsOn(RestoreTools)
        .Requires(() => SqlServerConnectionString)
        .Executes(() =>
        {
            var dbContextProject = Solution.GetAllProjects("Spenses.Resources.Relational").Single();

            EntityFrameworkDatabaseUpdate(s => s
                .SetProject(dbContextProject)
                .SetConnection(SqlServerConnectionString));

            DotNetRun(s => s
                .SetProjectFile(RelationalSetupTool)
                .SetApplicationArguments($"views --connection \"{SqlServerConnectionString}\""));
        });

    Target IntegrationTestSetup => _ => _
        .DependsOn(MigrateDatabase)
        .Requires(() => SqlServerConnectionString)
        .Executes(() =>
        {
            DotNetRun(s => s
                .SetProjectFile(RelationalSetupTool)
                .SetApplicationArguments($"seed --connection \"{SqlServerConnectionString}\""));
        });

    Target IntegrationTest => _ => _
        .DependsOn<IRestore>()
        .DependsOn(IntegrationTestSetup)
        .Requires(() => SqlServerConnectionString)
        .Produces(this.FromComponent<IReportCoverage>().CoverageReportDirectory / "*.trx")
        .Produces(this.FromComponent<IReportCoverage>().CoverageReportDirectory / "*.xml")
        .Executes(() =>
        {
            var integrationTestProjects = Solution.GetAllProjects("*.IntegrationTests");

            DotNetTest(_ => _
                .Apply(this.FromComponent<ITest>().TestSettingsBase)
                .Apply(TestSettings)
                .SetVerbosity(DotNetVerbosity.Minimal)
                .SetProcessEnvironmentVariable("Spenses:SqlServer:ConnectionString", SqlServerConnectionString)
                .CombineWith(integrationTestProjects, (_, v) => _
                    .Apply(this.FromComponent<ITest>().TestProjectSettingsBase, v)),
                completeOnFailure: true);
        });

    Target IReportCoverage.ReportCoverage => _ => _
        .Inherit<IReportCoverage>()
        .TriggeredBy(IntegrationTest)
        .Consumes(IntegrationTest);
}
