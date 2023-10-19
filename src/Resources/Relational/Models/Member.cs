using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class Member : AggregateRoot
{
    public string Name { get; set; } = null!;

    public string? ContactEmail { get; set; }

    [Precision(3, 2)]
    [Range(0.00, 1.00)]
    public decimal DefaultSplitPercentage { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public string? UserId { get; set; }

    public UserIdentity? User { get; set; }

    public ICollection<Expense> PaidExpenses { get; set; } = new HashSet<Expense>();

    public ICollection<ExpenseShare> ExpenseShares { get; set; } = new HashSet<ExpenseShare>();

    public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}
