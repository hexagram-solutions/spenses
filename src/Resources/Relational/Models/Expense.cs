using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class Expense : AggregateRoot
{
    public string Description { get; set; } = null!;

    public DateOnly Date { get; set; }

    [Precision(10, 2)]
    public decimal Amount { get; set; }

    public Guid IncurredByMemberId { get; set; }

    public Member IncurredByMember { get; set; } = null!;

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;
}
