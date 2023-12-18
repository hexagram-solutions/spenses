using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.ComponentModel;
using Spenses.Resources.Relational.Models;

namespace Spenses.Resources.Relational.DigestModels;

[BaseTable(nameof(Payment), "p")]
[JoinedTable(JoinType.Left, nameof(Member), "pbm", $"pbm.Id = p.{nameof(Payment.PaidByMemberId)}")]
[JoinedTable(JoinType.Left, nameof(Member), "ptm", $"ptm.Id = p.{nameof(Payment.PaidToMemberId)}")]
[JoinedTable(JoinType.Left, nameof(ApplicationUser), "cui", $"cui.Id = p.{nameof(Payment.CreatedById)}")]
[JoinedTable(JoinType.Left, nameof(ApplicationUser), "mui", $"mui.Id = p.{nameof(Payment.ModifiedById)}")]
public class PaymentDigest
{
    public Guid Id { get; set; }

    public Guid HomeId { get; set; }

    public DateOnly Date { get; set; }

    [Precision(8, 2)]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    public string? Note { get; set; }

    [SourceColumn("pbm", nameof(Member.Id))]
    public Guid PaidByMemberId { get; set; }

    [SourceColumn("pbm", nameof(Member.Name))]
    public string PaidByMemberName { get; set; } = null!;

    [SourceColumn("ptm", nameof(Member.Id))]
    public Guid PaidToMemberId { get; set; }

    [SourceColumn("ptm", nameof(Member.Name))]
    public string PaidToMemberName { get; set; } = null!;

    [SourceColumn("cui", nameof(ApplicationUser.UserName))]
    public string CreatedByUserName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [SourceColumn("mui", nameof(ApplicationUser.UserName))]
    public string ModifiedByUserName { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }
}
