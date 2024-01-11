using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record CurrentUser
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string NickName { get; init; }

    [Required]
    public required bool EmailVerified { get; init; }
}
