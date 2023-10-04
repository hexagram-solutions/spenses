namespace Spenses.Resources.Relational.ComponentModel;

public enum ApplyFunctionType
{
    Outer
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ApplyFunction : Attribute
{
    public ApplyFunction(ApplyFunctionType applyFunctionType, string alias, string function)
    {
        ApplyFunctionType = applyFunctionType;
        Alias = alias;
        Function = function;
    }

    public ApplyFunctionType ApplyFunctionType { get; }

    public string Alias { get; }

    public string Function { get; }
}
