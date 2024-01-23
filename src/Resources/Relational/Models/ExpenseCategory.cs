namespace Spenses.Resources.Relational.Models;

public class ExpenseCategory : AggregateRoot
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDefault { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public ICollection<Expense> Expenses { get; set; } = [];
}
