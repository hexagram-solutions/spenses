using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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

    [Precision(10, 2)]
    public decimal AnnualTakeHomeIncome { get; set; }
}
