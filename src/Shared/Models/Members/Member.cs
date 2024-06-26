using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Users;

namespace Spenses.Shared.Models.Members;

public record Member : MemberProperties
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public MemberStatus Status { get; set; }

    [Required]
    public string AvatarUrl { get; set; } = null!;

    public User? User { get; set; }
}

public record CreateMemberProperties : MemberProperties
{
    [Required]
    public bool ShouldInvite { get; set; }
}

public record MemberProperties
{
    [Required]
    public string Name { get; set; } = null!;

    [EmailAddress]
    public string? ContactEmail { get; set; }

    [Required]
    [Range(0, 1)]
    public decimal DefaultSplitPercentage { get; set; }
}
