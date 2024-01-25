using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Models.Users;

namespace Spenses.Shared.Models.Invitations;

public record InvitationBase
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

public record InvitationProperties : InvitationBase
{
    public Guid? MemberId { get; set; }
}

public record Invitation : InvitationBase
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public InvitationStatus Status { get; set; }

    public Member? Member { get; set; }

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}
