using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class Credit : AggregateRoot
{
    public DateOnly Date { get; set; }

    [Precision(10, 2)]
    public decimal Amount { get; set; }

    public Guid PaidByMemberId { get; set; }

    public Member PaidByMember { get; set; } = null!;

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;
}
