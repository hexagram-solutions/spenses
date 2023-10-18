using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models.Members;

public record Member : MemberProperties
{
    [Required]
    public Guid Id { get; set; }
}

public record MemberProperties
{
    [Required]
    public string Name { get; set; } = null!;

    [EmailAddress]
    public string? ContactEmail { get; set; }

    [Required]
    [Range(0, 1)]
    public double DefaultSplitPercentage { get; set; }
}
