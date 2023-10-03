using System.ComponentModel.DataAnnotations;

namespace Spenses.Resources.Relational.Models;

public class Member : AggregateRoot
{
    public string Name { get; set; } = null!;

    [Range(0, 1)]
    public double SplitPercentage { get; set; }

    public Guid HomeId { get; set; }

    public Home Home { get; set; } = null!;

    public string? UserId { get; set; }

    public UserIdentity? User { get; set; }

    public ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();

    public ICollection<Credit> Credits { get; set; } = new HashSet<Credit>();
}
