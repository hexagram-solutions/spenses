namespace Spenses.Shared.Common;

public static class ConfigConstants
{
    public const string AzureCommunicationServicesConnectionString = "Spenses:Azure:CommunicationServices:ConnectionString";
    public const string AzureStorageAccountConnectionString = "Spenses:Azure:StorageAccount:ConnectionString";

    public const string CommunicationEmailConfigurationSection = "Spenses:Communication:Email";
    public const string CommunicationSmtpOptionsSection = "Spenses:Communication:Email:Smtp";

    public const string SqlServerConnectionString = "Spenses:SqlServer:ConnectionString";

    public const string SpensesApiAllowedOrigins = "Spenses:Api:AllowedOrigins";
    public const string SpensesApiBaseUrl = "Spenses:Api:BaseUrl";

    public const string SpensesIdentityEmailConfigurationSection = "Spenses:Identity:Email";

    public const string SpensesLoggingLongRunningRequestThreshold = "Spenses:Logging:LongRunningRequestThreshold";

    public const string SpensesAppConfigurationConnectionString = "Spenses-AppConfiguration-ConnectionString";
    public const string SpensesAppConfigurationSentinel = "Spenses:AppConfiguration:Sentinel";

    public const string SpensesFeaturesErrorGenerationFrequency = "Spenses:Features:ErrorGeneration:Frequency";

    public const string SpensesDataProtectionApplicationName = "Spenses:DataProtection:ApplicationName";
    public const string SpensesDataProtectionKeyIdentifier = "Spenses:DataProtection:KeyIdentifier";
    public const string SpensesDataProtectionBlobStorageContainerName = "Spenses:DataProtection:BlobContainerName";
    public const string SpensesDataProtectionBlobStorageBlobName = "Spenses:DataProtection:BlobName";

    public const string SpensesTestDefaultUserPassword = "Spenses:Test:DefaultUserPassword";
    public const string SpensesTestSystemUserId = "Spenses:Test:SystemUserId";
    public const string SpensesTestIntegrationTestUserId = "Spenses:Test:IntegrationTestUserId";
    public const string SpensesTestIntegrationTestUserEmail = "Spenses:Test:IntegrationTestUserEmail";
}
