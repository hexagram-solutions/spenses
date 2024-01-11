using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record CurrentUser
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string NickName { get; set; } = null!;

    [Required]
    public bool EmailVerified { get; set; }
}
