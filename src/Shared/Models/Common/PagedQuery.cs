using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spenses.Shared.Models.Common;

public record PagedQuery<TResult>
{
    [Description("The results page number")]
    public int? Skip { get; set; }

    [Description("The number of results per page")]
    [Range(1, 200)]
    public int? Take { get; set; }

    [Description("The property to order results by")]
    public string? OrderBy { get; set; }

    [Description("The sort direction")]
    public SortDirection? SortDirection { get; set; }
}

public record PagedResult<TResult>(int TotalCount, IEnumerable<TResult> Items);
