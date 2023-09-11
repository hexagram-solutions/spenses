using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Common.Results;
using Spenses.Application.Homes;
using Spenses.Application.Models;
using Spenses.Web.Infrastructure;

namespace Spenses.Web.Pages.Homes;

public class DetailsModel : PageModel
{
    private readonly IMediator _mediator;

    public DetailsModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Home Home { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        var homeResult = await _mediator.Send(new HomeQuery(id.GetValueOrDefault()));

        if (!homeResult.IsSuccess)
            (homeResult.Result as ErrorResult)!.ToActionResult();

        Home = homeResult.Value;

        return Page();
    }
}
