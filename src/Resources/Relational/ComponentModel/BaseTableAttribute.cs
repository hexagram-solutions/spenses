namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class BaseTableAttribute : Attribute
{
    private const string DefaultAlias = "x";

    public BaseTableAttribute(string tableName, string? alias = null)
    {
        TableName = tableName;
        Alias = alias ?? DefaultAlias;
    }

    public string TableName { get; }

    public string Alias { get; }
}
