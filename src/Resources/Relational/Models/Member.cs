using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class Member : AggregateRoot
{
    public string Name { get; set; } = null!;

    public string? ContactEmail { get; set; }

    [Precision(5, 4)]
    [Range(0.00, 1.00)]
    public decimal DefaultSplitPercentage { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public string? UserId { get; set; }

    public UserIdentity? User { get; set; }

    public ICollection<Expense> ExpensesPaid { get; set; } = new HashSet<Expense>();

    public ICollection<ExpenseShare> ExpenseShares { get; set; } = new HashSet<ExpenseShare>();

    public ICollection<Payment> PaymentsPaid { get; set; } = new HashSet<Payment>();

    public ICollection<Payment> PaymentsReceived { get; set; } = new HashSet<Payment>();
}
