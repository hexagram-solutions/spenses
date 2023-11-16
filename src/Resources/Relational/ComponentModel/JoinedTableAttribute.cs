namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class JoinedTableAttribute(JoinType joinType, string tableName, string tableAlias, string condition) : Attribute
{
    public JoinType JoinType { get; } = joinType;

    public string TableName { get; } = tableName;

    public string Alias { get; } = tableAlias;

    public string Condition { get; } = condition;
}
