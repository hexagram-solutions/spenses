using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;

    [Required]
    public string DisplayName { get; set; } = null!;

    public string? InvitationToken { get; set; }

    public void Deconstruct(out string email, out string password, out string displayName, out string? invitationToken)
    {
        (email, password, displayName, invitationToken) = (Email, Password, DisplayName, InvitationToken);
    }
}
