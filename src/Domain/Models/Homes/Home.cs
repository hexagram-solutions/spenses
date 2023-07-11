namespace Spenses.Domain.Models.Homes;

public record Home : HomeProperties
{
    public Guid Id { get; set; }
}

public record HomeProperties
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
