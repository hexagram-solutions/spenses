using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Me;

public record UserProfileProperties
{
    [Required]
    public string DisplayName { get; set; } = null!;
}

public record CurrentUser : UserProfileProperties
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public bool EmailVerified { get; set; }
}
