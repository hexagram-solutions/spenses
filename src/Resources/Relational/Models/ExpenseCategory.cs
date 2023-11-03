namespace Spenses.Resources.Relational.Models;

public class ExpenseCategory : AggregateRoot
{
    public string Name { get; set; } = null!;

    public string? Note { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();
}
