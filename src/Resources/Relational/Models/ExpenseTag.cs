namespace Spenses.Resources.Relational.Models;

public class ExpenseTag
{
    public string Name { get; set; } = null!;

    public Guid ExpenseId { get; set; }

    public Expense Expense { get; set; } = null!;
}
