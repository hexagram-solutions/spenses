namespace Spenses.Resources.Relational.ComponentModel;

[AttributeUsage(AttributeTargets.Property)]
public sealed class CalculatedColumnAttribute : Attribute
{
    public CalculatedColumnAttribute(string sql)
    {
        Sql = sql;
    }

    public string Sql { get; }
}
