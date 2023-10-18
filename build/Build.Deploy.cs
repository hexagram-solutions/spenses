using System.Linq;
using Hexagrams.Nuke.Components;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable InconsistentNaming

partial class Build
{
    [PathVariable]
    readonly Tool Az;

    [PathVariable]
    readonly Tool Npx;

    Target Publish => _ => _
        .Description("Publish deployment artifacts.")
        .Executes(() =>
        {
            DotNetPublish(_ => _
                .SetProject(ApiProject)
                .SetConfiguration("Release")
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetProcessArgumentConfigurator(args => args
                    .Add("/t:PublishContainer")
                    .Add("--os linux")
                    .Add("--arch x64")));
        });

    Target AzureLogin => _ => _
        .Description("Log in with the Azure CLI.")
        .Requires(
            () => AzureUsername,
            () => AzurePassword,
            () => AzureTenant)
        .Executes(() =>
        {
            Az("login --service-principal " +
                $"-u {AzureUsername} " +
                $"-p {AzurePassword} " +
                $"--tenant {AzureTenant}");
        });

    Project ApiProject => Solution.GetAllProjects("Spenses.Api").Single();

    string DockerTag => IsServerBuild ? GitVersion.NuGetVersionV2 : "dev";

    string DockerImageName => $"spenses-api:{DockerTag}";

    Target PushDockerImage => _ => _
        .Description("Push docker images to the registry.")
        .DependsOn(AzureLogin)
        .Executes(() =>
        {
            var dockerFilePath = ApiProject.Directory / "Dockerfile";

            Az("acr build " +
                $"--registry {ContainerRegistryServer} " +
                $"--image {DockerImageName} " +
                $"--file {dockerFilePath} {RootDirectory}");
        });

    Target DeployApi => _ => _
        .Description("Deploy the API service to Azure Container Apps.")
        .DependsOn(PushDockerImage)
        .Requires(
            () => ContainerRegistryServer,
            () => ContainerRegistryUsername,
            () => ContainerRegistryPassword)
        .Executes(() =>
        {
            var containerAppImage = $"{ContainerRegistryServer}/{DockerImageName}";
            var containerAppName = "ca-spenses-test";

            Az("containerapp up " +
                $"--name {containerAppName} " +
                $"--resource-group {AzureResourceGroup} " +
                $"--environment {ContainerAppEnvironment} " +
                $"--image {containerAppImage} " +
                "--target-port 80 " +
                "--ingress external " +
                $"--registry-server {ContainerRegistryServer} " +
                $"--registry-username {ContainerRegistryUsername} " +
                $"--registry-password {ContainerRegistryPassword}");
        });

    Project WebProject => Solution.GetAllProjects("Spenses.Client.Web").Single();

    Target DeployWebApp => _ => _
        .Description("Deploy the front-end web app to Azure Static Web Apps.")
        .DependsOn<IClean>()
        .Requires(
            () => AzureStaticWebAppsApiToken)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(WebProject)
                .SetFramework("net7.0")
                .SetConfiguration(Configuration.Release));

            Npx($"@azure/static-web-apps-cli deploy " +
                $"{WebProject.Directory / "bin" / "Release" / "net7.0" / "publish" / "wwwroot"} " +
                $"--deployment-token {AzureStaticWebAppsApiToken} " +
                $"--env Production");
        });
}
