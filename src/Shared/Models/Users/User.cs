using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Users;

public record User
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    public string DisplayName { get; init; } = null!;
}
