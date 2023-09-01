using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class Expense : Entity
{
    public string Description { get; set; } = null!;

    public DateOnly Date { get; set; }

    [Precision(10, 2)]
    public decimal Amount { get; set; }

    public Member Member { get; set; } = null!;

    public Home Home { get; set; } = null!;
}
