using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Me;

public record ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = null!;

    public void Deconstruct(out string oldPassword, out string newPassword)
    {
        (oldPassword, newPassword)= (CurrentPassword, NewPassword);
    }
}
