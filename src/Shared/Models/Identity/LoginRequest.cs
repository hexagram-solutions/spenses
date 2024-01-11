using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public void Deconstruct(out string email, out string password)
    {
        (email, password) = (Email, Password);
    }
}
