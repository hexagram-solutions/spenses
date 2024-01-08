using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Authentication;

public record CurrentUser
{
    [Required]
    public required string UserName { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required bool EmailVerified { get; init; }
}
