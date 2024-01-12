using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string ResetCode { get; set; } = null!;

    [Required]
    public string NewPassword { get; set; } = null!;

    public void Deconstruct(out string email, out string resetCode, out string newPassword)
    {
        (email, resetCode, newPassword) = (Email, ResetCode, NewPassword);
    }
}
