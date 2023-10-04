using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.ComponentModel;
using Spenses.Resources.Relational.Models;

namespace Spenses.Resources.Relational.DigestModels;

[BaseTable(nameof(Expense), "e")]
[JoinedTable(JoinType.Left, nameof(Member), "ibm", "ibm.Id = e.IncurredByMemberId")]
[JoinedTable(JoinType.Left, nameof(UserIdentity), "cui", "e.CreatedById = cui.Id")]
[JoinedTable(JoinType.Left, nameof(UserIdentity), "mui", "e.ModifiedById = mui.Id")]
public class ExpenseDigest
{
    public Guid Id { get; set; }

    public Guid HomeId { get; set; }

    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    [Precision(8, 2)]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    [SourceColumn("ibm", nameof(Member.Name))]
    public string IncurredByMemberName { get; set; } = null!;

    [CalculatedColumn("SELECT STRING_AGG(t.Name, ' ') FROM ExpenseTag t WHERE t.ExpenseId = e.Id")]
    public string? Tags { get; set; }

    [SourceColumn("cui", nameof(UserIdentity.NickName))]
    public string CreatedByUserName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [SourceColumn("mui", nameof(UserIdentity.NickName))]
    public string ModifiedByUserName { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }
}
