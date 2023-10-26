using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models.Common;

public record PagedQuery<TResult>
{
    [Description("The results page number")]
    [DefaultValue(0)]
    [Required]
    public int Skip { get; set; }

    [Description("The number of results per page")]
    [DefaultValue(25)]
    [Range(1, 200)]
    [Required]
    public int Take { get; set; } = 25;

    [Description("The property to order results by")]
    public string? OrderBy { get; set; }

    [Description("The sort direction")]
    public SortDirection? SortDirection { get; set; }
}

public record PagedResult<TResult>(int TotalCount, IEnumerable<TResult> Items);
