namespace Spenses.Resources.Relational.Models;

public class Home : AggregateRoot
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<Member> Members { get; set; } = [];

    public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = [];

    public ICollection<Expense> Expenses { get; set; } = [];

    public ICollection<Payment> Payments { get; set; } = [];
}
