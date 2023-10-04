namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class JoinedTableAttribute : Attribute
{
    public JoinedTableAttribute(JoinType joinType, string tableName, string tableAlias, string condition)
    {
        JoinType = joinType;
        TableName = tableName;
        Alias = tableAlias;
        Condition = condition;
    }

    public JoinType JoinType { get; }

    public string TableName { get; }

    public string Alias { get; }

    public string Condition { get; }
}
