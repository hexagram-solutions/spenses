using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    public void Deconstruct(out string email, out string password, out string nickName)
    {
        (email, password, nickName) = (Email, Password, Name);
    }
}
