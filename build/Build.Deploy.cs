using Nuke.Common;
using static Nuke.Common.Tools.PowerShell.PowerShellTasks;

partial class Build
{
    Target AzureLogin => _ => _
        .Description("Log in with the Azure CLI.")
        .Requires(
            () => AzureUsername,
            () => AzurePassword,
            () => AzureTenant)
        .Executes(() =>
        {
            PowerShell($"az login --service-principal " +
                $"-u {AzureUsername} " +
                $"-p {AzurePassword} " +
                $"--tenant {AzureTenant}");
        });

    Target PushDockerImage => _ => _
        .Description("Push docker images to the registry.")
        .DependsOn(AzureLogin)
        .Executes(() =>
        {
            var acrName = "crspensestest";

            var dockerFilePath = ApiProject.Directory / "Dockerfile";

            PowerShell("az acr build " +
                $"--registry {acrName} " +
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

            PowerShell("az containerapp up " +
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
