using Microsoft.Extensions.Configuration;

namespace Spenses.Common.Configuration;

/// <summary>
/// Extension methods for <see cref="IConfigurationBuilder"/>.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Add new configuration keys for each existing configuration key, substituting delimiters with those supplied.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <param name="keyDelimiters">The configuration setting delimiters.</param>
    /// <returns>The configuration builder.</returns>
    /// <remarks>
    /// Given the <paramref name="keyDelimiters" />: <c>":", "_", "-"</c> an existing key of <c>Foo:Bar</c> will
    /// have duplicate values added for the keys <c>"Foo_Bar"</c> and <c>"Foo-Bar"</c>.
    /// <br /><br />
    /// It is important that this is the last configuration added in the configuration chain. Any configuration
    /// keys that are added afterward will not have their delimiters substituted.
    /// </remarks>
    public static IConfigurationBuilder SetKeyDelimiters(this IConfigurationBuilder builder,
        params string[] keyDelimiters)
    {
        var intermediateConfig = builder.Build();

        builder.Add(new DelimitedConfigurationSource(keyDelimiters, intermediateConfig));

        return builder;
    }
}
