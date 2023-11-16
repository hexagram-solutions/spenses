namespace Spenses.Resources.Relational.ComponentModel;

public enum ApplyFunctionType
{
    Outer
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ApplyFunction(ApplyFunctionType applyFunctionType, string alias, string function) : Attribute
{
    public ApplyFunctionType ApplyFunctionType { get; } = applyFunctionType;

    public string Alias { get; } = alias;

    public string Function { get; } = function;
}
