namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SourceColumnAttribute(string tableAlias, params string[] columnNameParts) : Attribute
{
    public string TableAlias { get; } = tableAlias;

    public string? ColumnName { get; } = columnNameParts.Length > 0 ? string.Join("_", columnNameParts) : null;
}
