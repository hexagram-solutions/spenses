using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Common.Results;
using Spenses.Application.Homes;
using Spenses.Application.Models.Homes;
using Spenses.Web.Infrastructure;

namespace Spenses.Web.Pages.Homes;

public class DeleteModel : PageModel
{
    private readonly IMediator _mediator;

    public DeleteModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public Home Home { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id) // todo: why is id nullable?
    {
        var homeResult = await _mediator.Send(new HomeQuery(id.GetValueOrDefault()));

        if (!homeResult.IsSuccess)
            (homeResult.Result as ErrorResult)!.ToActionResult();

        Home = homeResult.Value;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id) // todo: why is id nullable? because it's a query param. how to make into path
    {
        var result = await _mediator.Send(new DeleteHomeCommand(id.GetValueOrDefault()));

        if (!result.IsSuccess)
            (result as ErrorResult)!.ToActionResult();

        return RedirectToPage("./Index");
    }
}
