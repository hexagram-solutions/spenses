namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SortOverrideAttribute : Attribute
{
    public SortOverrideAttribute(params string[] paths)
    {
        Paths = paths;
    }

    public string[] Paths { get; }
}
