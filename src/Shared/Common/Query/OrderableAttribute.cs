namespace Spenses.Shared.Common.Query;

/// <summary>
/// Marks a property as one that can be used to order query results.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class OrderableAttribute : Attribute
{
}
