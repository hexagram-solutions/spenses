namespace Spenses.Api.IntegrationTests;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
public class IdentityWebApplicationCollection : ICollectionFixture<IdentityWebApplicationFixture>
{
    public const string CollectionName = "Identity Web Application";

    // This class has no code, and is never created. Its purpose is simply to be the place to apply
    // [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}
