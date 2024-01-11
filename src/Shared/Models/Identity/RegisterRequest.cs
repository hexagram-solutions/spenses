using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Identity;

public record RegisterRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public required string NickName { get; set; }

    public void Deconstruct(out string email, out string password, out string nickName)
    {
        (email, password, nickName) = (Email, Password, NickName);
    }
}
