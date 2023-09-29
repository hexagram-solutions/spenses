using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.PowerShell.PowerShellTasks;

partial class Build
{
    [PathVariable]
    readonly Tool Az;

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
        // .DependsOn(AzureLogin)
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
}
