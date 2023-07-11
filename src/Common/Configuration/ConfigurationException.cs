namespace Spenses.Common.Configuration;

/// <summary>
/// Represents errors that occur during application configuration.
/// </summary>
public class ConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException" /> class with the specified
    /// <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The configuration error message.</param>
    public ConfigurationException(string message)
        : base(message)
    {
    }
}
