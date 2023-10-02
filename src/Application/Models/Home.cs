using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public record Home : HomeProperties, IAggregateRoot
{
    public Guid Id { get; set; }

    public Member[] Members { get; set; } = Array.Empty<Member>();

    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public User ModifiedBy { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }
}

public record HomeProperties
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
