using Microsoft.Extensions.Configuration;

namespace Spenses.Common.Configuration;

internal class DelimitedConfigurationSource : IConfigurationSource
{
    private readonly IEnumerable<string> _keyDelimiters;
    private readonly IConfiguration _config;

    public DelimitedConfigurationSource(IEnumerable<string> keyDelimiters, IConfiguration config)
    {
        _keyDelimiters = keyDelimiters;
        _config = config;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DelimitedConfigurationProvider(_keyDelimiters, _config);
    }
}
