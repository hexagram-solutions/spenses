using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Members;

public record Member : MemberProperties
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public bool IsActive { get; set; }
}

public record MemberProperties
{
    [Required]
    public string Name { get; set; } = null!;

    [EmailAddress]
    public string? ContactEmail { get; set; }

    [Required]
    [Range(0, 1)]
    public decimal DefaultSplitPercentage { get; set; }
}
