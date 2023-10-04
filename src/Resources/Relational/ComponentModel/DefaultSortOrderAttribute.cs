namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DefaultSortOrderAttribute : Attribute
{
    public DefaultSortOrderAttribute()
    {
    }

    public DefaultSortOrderAttribute(params string[] properties)
    {
        if (properties == null)
            throw new ArgumentNullException(nameof(properties));

        Paths = properties.Select(p => p.Split('.')).ToArray();
    }

    public string[][] Paths { get; set; } = Array.Empty<string[]>();
}
