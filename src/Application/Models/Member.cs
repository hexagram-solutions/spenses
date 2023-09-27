using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public record Member : MemberProperties, IAggregateRoot
{
    [Required]
    public Guid Id { get; set; }

    public User? User { get; set; }

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}

public record MemberProperties
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal AnnualTakeHomeIncome { get; set; }
}
