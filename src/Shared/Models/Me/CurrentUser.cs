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
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public bool EmailVerified { get; set; }

    [Required]
    public string AvatarUrl { get; set; } = null!;
}
