using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class Member : AggregateRoot
{
    public string Name { get; set; } = null!;

    [Precision(10, 2)]
    public decimal AnnualTakeHomeIncome { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public string? UserId { get; set; }

    public UserIdentity? User { get; set; }

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
