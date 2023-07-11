using Microsoft.Extensions.Configuration;

namespace Spenses.Common.Configuration;

internal class DelimitedConfigurationProvider : ConfigurationProvider
{
    private readonly IEnumerable<string> _keyDelimiters;
    private readonly IConfiguration _config;

    public DelimitedConfigurationProvider(IEnumerable<string> keyDelimiters, IConfiguration config)
    {
        _keyDelimiters = keyDelimiters;
        _config = config;
    }

    public override void Load()
    {
        var existingItems = new Dictionary<string, string>(_config.AsEnumerable()!);

        foreach (var setting in existingItems)
        {
            var normalizedKeys = NormalizeKey(setting.Key);

            foreach (var normalizedKey in normalizedKeys)
            {
                if (Data.ContainsKey(normalizedKey))
                    continue;

                Data.Add(normalizedKey, setting.Value);
            }
        }
    }

    private IEnumerable<string> NormalizeKey(string key)
    {
        var newKeys = new List<string>();

        foreach (var delimiter in _keyDelimiters)
            newKeys.AddRange(_keyDelimiters.Select(d => key.Replace(delimiter, d)).Distinct());

        return newKeys;
    }
}
