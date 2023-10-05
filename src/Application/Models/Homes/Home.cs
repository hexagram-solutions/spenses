using System.ComponentModel.DataAnnotations;
using Spenses.Application.Models.Members;
using Spenses.Application.Models.Users;

namespace Spenses.Application.Models.Homes;

public record Home : HomeProperties, IAggregateRoot
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Member[] Members { get; set; } = Array.Empty<Member>();

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}

public record HomeProperties
{
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public ExpensePeriod ExpensePeriod { get; set; }
}
