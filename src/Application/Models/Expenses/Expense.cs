using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Spenses.Application.Common.Serialization;
using Spenses.Application.Models.ExpenseCategories;
using Spenses.Application.Models.Members;
using Spenses.Application.Models.Users;

namespace Spenses.Application.Models.Expenses;

public abstract record ExpenseBase
{
    public string? Note { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    [JsonConverter(typeof(LowerCaseNormalizingStringArrayConverter))]
    public string[] Tags { get; set; } = Array.Empty<string>();
}

public record ExpenseProperties : ExpenseBase
{
    [Required]
    public Guid PaidByMemberId { get; set; }

    public Guid? CategoryId { get; set; }
}

public record Expense : ExpenseBase
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Member PaidByMember { get; set; } = null!;

    public ExpenseCategory? Category { get; set; }

    public ExpenseShare[] ExpenseShares { get; set; } = Array.Empty<ExpenseShare>();

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}
