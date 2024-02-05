using System.Collections.Generic;
using System.Linq;
using Hexagrams.Extensions.Common;
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

    public Configure<DotNetTestSettings> TestSettings => s => s
        .SetExcludeByFile("\\\"*.Generated.cs,**/Resources/Relational/**.cs\\\"");

    Target RestoreTools => t => t
        .Executes(() =>
        {
            DotNetToolRestore(s => s);
        });

    Project RelationalSetupTool => Solution.GetAllProjects("Spenses.Tools.Setup").Single();

    Target MigrateDatabase => t => t
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

    IEnumerable<Project> IntegrationTestProjects => Solution.GetAllProjects("*.IntegrationTests");

    Target IntegrationTestSetup => t => t
        .DependsOn(MigrateDatabase)
        .Requires(() => SqlServerConnectionString)
        .Requires(() => IntegrationTestDefaultUserPassword)
        .Executes(() =>
        {
            var projectsToConfigure = IntegrationTestProjects.Concat(RelationalSetupTool.Yield());

            foreach (var project in projectsToConfigure)
            {
                DotNet("user-secrets " +
                    $"set Spenses:Test:DefaultUserPassword {IntegrationTestDefaultUserPassword} " +
                    $"--project {project}");
            }

            DotNetRun(s => s
                .SetProjectFile(RelationalSetupTool)
                .SetApplicationArguments($"seed --connection \"{SqlServerConnectionString}\""));
        });

    Target IntegrationTest => t => t
        .DependsOn<IRestore>()
        .DependsOn(IntegrationTestSetup)
        .Requires(() => SqlServerConnectionString)
        .Produces(this.FromComponent<IReportCoverage>().CoverageReportDirectory / "*.trx")
        .Produces(this.FromComponent<IReportCoverage>().CoverageReportDirectory / "*.xml")
        .Executes(() =>
        {
            DotNetTest(s => s
                .Apply(this.FromComponent<ITest>().TestSettingsBase)
                .Apply(TestSettings)
                .SetVerbosity(DotNetVerbosity.minimal)
                .SetProcessEnvironmentVariable("Spenses:SqlServer:ConnectionString", SqlServerConnectionString)
                .CombineWith(IntegrationTestProjects, (x, v) => x
                    .Apply(this.FromComponent<ITest>().TestProjectSettingsBase, v)),
                completeOnFailure: true);
        });

    Target IReportCoverage.ReportCoverage => t => t
        .Inherit<IReportCoverage>()
        .TriggeredBy(IntegrationTest)
        .Consumes(IntegrationTest);
}
