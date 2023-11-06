using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Spenses.Resources.Relational.Models;

public class ExpenseShare : Entity
{
    [Precision(8, 2)]
    [Range(0, 999_999.99)]
    public decimal OwedAmount { get; set; }

    [Precision(5, 4)]
    [Range(0.00, 1.00)]
    public decimal OwedPercentage { get; set; }

    public Guid ExpenseId { get; set; }

    public Expense Expense { get; set; } = null!;

    public Guid OwedByMemberId { get; set; }

    public Member OwedByMember { get; set; } = null!;
}
