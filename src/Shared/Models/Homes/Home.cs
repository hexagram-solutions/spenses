using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Models.Users;

namespace Spenses.Shared.Models.Homes;

public record Home : HomeProperties
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
}
