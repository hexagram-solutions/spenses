using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models.Common;

public record PagedQuery<TResult>
{
    [Description("The results page number")]
    [DefaultValue(1)]
    [Required]
    public int PageNumber { get; set; } = 1;

    [Description("The number of results per page")]
    [DefaultValue(50)]
    [Range(1, 200)]
    [Required]
    public int PageSize { get; set; } = 50;

    [Description("The property to order results by")]
    public string? OrderBy { get; set; }

    [Description("The sort direction")]
    public SortDirection? SortDirection { get; set; }
}

public record PagedResult<TResult>(int TotalCount, IEnumerable<TResult> Items);
