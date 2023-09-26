using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class Member : Entity
{
    public string Name { get; set; } = null!;

    [Precision(10, 2)]
    public decimal AnnualTakeHomeIncome { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
