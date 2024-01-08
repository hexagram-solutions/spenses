namespace Spenses.Shared.Models.ExpenseCategories;

public record ExpenseCategory : ExpenseCategoryProperties
{
    public Guid Id { get; set; }

    public bool IsDefault { get; set; }
}

public record ExpenseCategoryProperties
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
