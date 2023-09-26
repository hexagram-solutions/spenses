namespace Spenses.Application.Models;

public record Expense : ExpenseProperties
{
    public Guid Id { get; set; }
}

public record ExpenseProperties
{
    public string Description { get; set; } = null!;

    public DateOnly Date { get; set; }
    
    public decimal Amount { get; set; }

    public string[] Tags { get; set; } = Array.Empty<string>();

    public Guid IncurredByMemberId { get; set; }
}
