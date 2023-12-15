namespace Spenses.Application.Common;
public static class ConfigConstants
{
    public const string AspNetCoreEnvironment = "ENVIRONMENT";

    public const string SqlServerConnectionString = "Spenses:SqlServer:ConnectionString";

    public const string SpensesApiAllowedOrigins = "Spenses:Api:AllowedOrigins";
    public const string SpensesApiBaseUrl = "Spenses:Api:BaseUrl";
    public const string SpensesLoggingLongRunningRequestThreshold = "Spenses:Logging:LongRunningRequestThreshold";

    public const string SpensesAppConfigurationConnectionString = "Spenses-AppConfiguration-ConnectionString";
    public const string SpensesAppConfigurationSentinel = "Spenses:AppConfiguration:Sentinel";

    public const string SpensesOpenIdAuthority = "Spenses:OpenId:Authority";
    public const string SpensesOpenIdAudience = "Spenses:OpenId:Audience";
    public const string SpensesOpenIdClientId = "Spenses:OpenId:ClientId";

    public const string SpensesFeaturesErrorGenerationFrequency = "Spenses:Features:ErrorGeneration:Frequency";

    public const string SpensesDataProtectionApplicationName = "spenses";
}
