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

    Project ApiProject => Solution.GetAllProjects("Spenses.Api").Single();

    Project WebProject => Solution.GetAllProjects("Spenses.App").Single();

    Target Publish => t => t
        .Description("Publish deployment artifacts.")
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(ApiProject)
                .SetConfiguration("Release")
                .SetVersion(GitVersion.NuGetVersionV2)
                .AddProperty("PublishProfile", "DefaultContainer")
                .AddProperty("ContainerImageTags", GitVersion.NuGetVersionV2));
        });

    Target AzureLogin => t => t
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

    string DockerTag => IsServerBuild ? GitVersion.NuGetVersionV2 : "dev";

    string DockerImageName => $"spenses-api:{DockerTag}";

    Target PushDockerImage => t => t
        .Description("Push docker images to the registry.")
        .DependsOn(AzureLogin)
        .Requires(() => ContainerRegistryServer)
        .Executes(() =>
        {
            var dockerFilePath = ApiProject.Directory / "Dockerfile";

            Az("acr build " +
                $"--registry {ContainerRegistryServer} " +
                $"--image {DockerImageName} " +
                $"--file {dockerFilePath} {RootDirectory}");
        });

    Target DeployApi => t => t
        .Description("Deploy the API service to Azure Container Apps.")
        .DependsOn(PushDockerImage)
        .Requires(
            () => AzureResourceGroup,
            () => ContainerRegistryServer,
            () => ContainerRegistryUsername,
            () => ContainerRegistryPassword,
            () => ContainerAppEnvironment)
        .Executes(() =>
        {
            var containerAppImage = $"{ContainerRegistryServer}/{DockerImageName}";
            var containerAppName = "ca-spenses-test";

            Az("containerapp up " +
                $"--name {containerAppName} " +
                $"--resource-group {AzureResourceGroup} " +
                $"--environment {ContainerAppEnvironment} " +
                $"--image {containerAppImage} " +
                "--target-port 8080 " +
                "--ingress external " +
                $"--registry-username {ContainerRegistryUsername} " +
                $"--registry-password {ContainerRegistryPassword}");
        });

    Target DeployWebApp => t => t
        .Description("Deploy the front-end web app to Azure Static Web Apps.")
        .DependsOn<IClean>()
        .Requires(
            () => AzureStaticWebAppsDeploymentToken)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(WebProject)
                .SetConfiguration(Configuration.Release)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer));

            Npx($"@azure/static-web-apps-cli deploy " +
                $"{WebProject.Directory / "bin" / "Release" / "net8.0" / "publish" / "wwwroot"} " +
                $"--deployment-token {AzureStaticWebAppsDeploymentToken} " +
                $"--env Production");
        });
}
