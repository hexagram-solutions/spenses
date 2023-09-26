using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public record Member : MemberProperties
{
    [Required]
    public Guid Id { get; set; }
}

public record MemberProperties
{
    [Required]
    public string Name { get; set; } = null!;
    
    public decimal AnnualTakeHomeIncome { get; set; }
}
