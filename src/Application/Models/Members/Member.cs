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

    [Required]
    [Range(0, 1)]
    public double SplitPercentage { get; set; }
}
