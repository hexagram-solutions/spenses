namespace Spenses.Application.Common;
public static class ConfigConstants
{
    public const string SqlServerConnectionString = "Spenses:SqlServer:ConnectionString";

    public const string SpensesApiAllowedOrigins = "Spenses:Api:AllowedOrigins";
    public const string SpensesApiBaseUrl = "Spenses:Api:BaseUrl";

    public const string SpensesAppConfigurationConnectionString = "Spenses-AppConfiguration-ConnectionString";
    public const string SpensesAppConfigurationSentinel = "Spenses:AppConfiguration:Sentinel";

    public const string SpensesConfigurationEnvironment = "Spenses:Configuration:Environment";

    public const string SpensesOpenIdAuthority = "Spenses:OpenId:Authority";
    public const string SpensesOpenIdAudience = "Spenses:OpenId:Audience";
    public const string SpensesOpenIdClientId = "Spenses:OpenId:ClientId";
}
