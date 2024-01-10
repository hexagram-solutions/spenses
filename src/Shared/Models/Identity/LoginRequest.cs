using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public void Deconstruct(out string email, out string password)
    {
        (email, password) = (Email, Password);
    }
}
