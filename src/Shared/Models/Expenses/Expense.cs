using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Spenses.Shared.Common.Serialization;
using Spenses.Shared.Models.ExpenseCategories;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Models.Users;

namespace Spenses.Shared.Models.Expenses;

public abstract record ExpenseBase
{
    public string? Note { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    [JsonConverter(typeof(LowerCaseNormalizingStringListConverter))]
    public List<string> Tags { get; set; } = [];
}

public record ExpenseProperties : ExpenseBase
{
    [Required]
    public Guid PaidByMemberId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}

public record Expense : ExpenseBase
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Member PaidByMember { get; set; } = null!;

    public ExpenseCategory Category { get; set; } = null!;

    public ExpenseShare[] ExpenseShares { get; set; } = [];

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}
