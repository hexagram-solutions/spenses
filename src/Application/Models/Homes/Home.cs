using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models.Homes;

public record Home : HomeProperties
{
    [Required]
    public Guid Id { get; set; }
}

public record HomeProperties
{
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
