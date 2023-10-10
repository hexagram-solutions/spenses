using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.ComponentModel;
using Spenses.Resources.Relational.Models;

namespace Spenses.Resources.Relational.DigestModels;

[BaseTable(nameof(Credit), "c")]
[JoinedTable(JoinType.Left, nameof(Member), "pbm", "pbm.Id = c.PaidByMemberId")]
[JoinedTable(JoinType.Left, nameof(UserIdentity), "cui", "cui.Id = c.CreatedById")]
[JoinedTable(JoinType.Left, nameof(UserIdentity), "mui", "mui.Id = c.ModifiedById")]
public class CreditDigest
{
    public Guid Id { get; set; }

    public Guid HomeId { get; set; }

    public DateOnly Date { get; set; }

    [Precision(8, 2)]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    public string? Note { get; set; }

    [SourceColumn("pbm", nameof(Member.Name))]
    public string PaidByMemberName { get; set; } = null!;

    [SourceColumn("cui", nameof(UserIdentity.NickName))]
    public string CreatedByUserName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [SourceColumn("mui", nameof(UserIdentity.NickName))]
    public string ModifiedByUserName { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }
}
