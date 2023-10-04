using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Spenses.Application.Models;

public record PagedQuery<TResult>
{
    [FromQuery(Name = "pageNumber")]
    [Description("The results page number")]
    [DefaultValue(1)]
    [Required]
    public int PageNumber { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    [Description("The number of results per page")]
    [DefaultValue(50)]
    [Range(1, 200)]
    [Required]
    public int PageSize { get; set; } = 50;

    [FromQuery(Name = "orderBy")]
    [Description("The property to order results by")]
    public string? OrderBy { get; set; }

    [FromQuery(Name = "sortDirection")]
    [Description("The sort direction")]
    public SortDirection? SortDirection { get; set; }
}

public record PagedResult<TResult>(int TotalCount, IEnumerable<TResult> Items);
