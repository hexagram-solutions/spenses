using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Models.Users;

namespace Spenses.Shared.Models.Invitations;

public record InvitationProperties
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

public record Invitation : InvitationProperties
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public InvitationStatus Status { get; set; }

    [Required]
    public Member Member { get; set; } = null!;

    //TODO: Remove members from home object
    [Required]
    public Home Home { get; set; } = null!;

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}
