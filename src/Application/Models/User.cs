using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public record User
{
    [Required]
    public string Id { get; init; } = null!;

    [Required]
    public string Name { get; init; } = null!;
}
