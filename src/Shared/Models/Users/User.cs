using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Users;

public record User
{
    [Required]
    public string Id { get; init; } = null!;

    [Required]
    public string Name { get; init; } = null!;
}
