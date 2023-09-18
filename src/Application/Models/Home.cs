using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public record Home : HomeProperties
{
    [Required]
    public Guid Id { get; set; }

    public Member[] Members { get; set; } = Array.Empty<Member>();
}

public record HomeProperties
{
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
