namespace Spenses.Resources.Relational.Models;

public class Home : AggregateRoot
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ExpensePeriod ExpensePeriod { get; set; }

    public ICollection<Member> Members { get; set; } = new HashSet<Member>();

    public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new HashSet<ExpenseCategory>();

    public ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();

    public ICollection<Credit> Credits { get; set; } = new HashSet<Credit>();
}
