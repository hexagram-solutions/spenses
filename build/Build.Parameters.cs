using Nuke.Common;

// ReSharper disable InconsistentNaming

partial class Build
{
    [Secret]
    [Parameter]
    readonly string SqlServerConnectionString =
        "Server=.;Database=Spenses;Trusted_Connection=True;Encrypt=False;";

    [Parameter]
    readonly string AzureUsername = "8c4f044b-4c79-4fc8-a74f-0363bd0c5e22";

    [Secret]
    [Parameter]
    readonly string AzurePassword;

    [Parameter]
    readonly string AzureTenant = "892b7660-6ccd-40d5-8170-fd943620a80e";

    [Parameter]
    readonly string AzureResourceGroup = "rg-spenses-test";

    [Parameter]
    readonly string ContainerAppEnvironment = "cae-spenses-test";

    [Parameter]
    readonly string ContainerRegistryServer = "crspensestest.azurecr.io";

    [Parameter]
    readonly string ContainerRegistryUsername = "crspensestest";

    [Secret]
    [Parameter]
    readonly string ContainerRegistryPassword;

    [Secret]
    [Parameter]
    readonly string AzureStaticWebAppsApiToken;

    [Secret]
    [Parameter]
    readonly string IntegrationTestDefaultUserPassword = "Password1!";
}
