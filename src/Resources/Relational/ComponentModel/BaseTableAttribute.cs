namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class BaseTableAttribute(string tableName, string? alias = null) : Attribute
{
    private const string DefaultAlias = "x";

    public string TableName { get; } = tableName;

    public string Alias { get; } = alias ?? DefaultAlias;
}
