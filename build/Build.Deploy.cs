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
                $"--username {AzureUsername} " +
                $"--password \"{AzurePassword}\" " +
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
        .Executes(() =>
        {
            var containerAppImage = $"{ContainerRegistryServer}/{DockerImageName}";

            PowerShell("az containerapp up " +
                "--name spenses-api " +
                $"--resource-group {AzureResourceGroup} " +
                $"--environment {ContainerAppEnvironment} " +
                $"--image {containerAppImage} " +
                "--target-port 80 " +
                "--ingress external " +
                $"--registry-server {ContainerRegistryServer}");
        });
}
