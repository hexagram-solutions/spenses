namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class CalculatedColumnAttribute(string sql) : Attribute
{
    public string Sql { get; } = sql;
}
