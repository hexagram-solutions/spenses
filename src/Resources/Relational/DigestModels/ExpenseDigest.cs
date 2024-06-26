using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.ComponentModel;
using Spenses.Resources.Relational.Models;

namespace Spenses.Resources.Relational.DigestModels;

[BaseTable(nameof(Expense), "e")]
[JoinedTable(JoinType.Left, nameof(Member), "pbm", $"pbm.Id = e.{nameof(Expense.PaidByMemberId)}")]
[JoinedTable(JoinType.Left, nameof(ExpenseCategory), "ec", $"ec.Id = e.{nameof(Expense.CategoryId)}")]
[JoinedTable(JoinType.Left, nameof(ApplicationUser), "cui", $"cui.Id = e.{nameof(Expense.CreatedById)}")]
[JoinedTable(JoinType.Left, nameof(ApplicationUser), "mui", $"mui.Id = e.{nameof(Expense.ModifiedById)}")]
public class ExpenseDigest
{
    public Guid Id { get; set; }

    public Guid HomeId { get; set; }

    public string? Note { get; set; }

    public DateOnly Date { get; set; }

    [Precision(8, 2)]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    [SourceColumn("ec", nameof(ExpenseCategory.Id))]
    public Guid? CategoryId { get; set; }

    [SourceColumn("ec", nameof(ExpenseCategory.Name))]
    public string? CategoryName { get; set; }

    [SourceColumn("pbm", nameof(Member.Id))]
    public Guid PaidByMemberId { get; set; }

    [SourceColumn("pbm", nameof(Member.Name))]
    public string PaidByMemberName { get; set; } = null!;

    [CalculatedColumn("SELECT STRING_AGG(t.Name, ' ') FROM ExpenseTag t WHERE t.ExpenseId = e.Id")]
    public string? Tags { get; set; }

    [SourceColumn("cui", nameof(ApplicationUser.UserName))]
    public string CreatedByUserName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [SourceColumn("mui", nameof(ApplicationUser.UserName))]
    public string ModifiedByUserName { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }
}
