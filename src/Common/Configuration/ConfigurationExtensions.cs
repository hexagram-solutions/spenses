using Microsoft.Extensions.Configuration;

namespace Spenses.Common.Configuration;

/// <summary>
/// Extension methods for <see cref="IConfiguration"/>.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Get a configuration value, throwing an exception if the key is not found the value is whitespace.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="ConfigurationException">The configuration value is not found or is whitespace.</exception>
    public static string Require(this IConfiguration config, string key)
    {
        var value = config[key];

        if (string.IsNullOrWhiteSpace(value))
            throw new ConfigurationException($"No value found for configuration key {key}");

        return value;
    }

    /// <summary>
    /// Get a configuration section's children as an array of values.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="section">The configuration section name.</param>
    /// <returns>The configuration values.</returns>
    public static string[] Collection(this IConfiguration config, string section)
    {
        return config.GetSection(section).GetChildren()
            .Select(x => x.Value!)
            .ToArray();
    }
}
