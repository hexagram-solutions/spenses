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

    public MemberStatus Status { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public Guid? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public ICollection<Expense> ExpensesPaid { get; set; } = [];

    public ICollection<ExpenseShare> ExpenseShares { get; set; } = [];

    public ICollection<Payment> PaymentsPaid { get; set; } = [];

    public ICollection<Payment> PaymentsReceived { get; set; } = [];

    public ICollection<Invitation> Invitations { get; set; } = [];
}
