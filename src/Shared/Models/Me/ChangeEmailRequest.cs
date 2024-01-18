using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Me;

public record ChangeEmailRequest
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; } = null!;
}
