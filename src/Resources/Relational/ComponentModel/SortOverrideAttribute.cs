namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SortOverrideAttribute(params string[] paths) : Attribute
{
    public string[] Paths { get; } = paths;
}
