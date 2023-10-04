namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SourceColumnAttribute : Attribute
{
    public SourceColumnAttribute(string tableAlias, params string[] columnNameParts)
    {
        TableAlias = tableAlias;
        ColumnName = columnNameParts.Length > 0 ? string.Join("_", columnNameParts) : null;
    }

    public string TableAlias { get; }

    public string? ColumnName { get; }
}
