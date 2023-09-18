using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Common.Results;
using Spenses.Application.Features.Homes;
using Spenses.Application.Models;
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

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var homeResult = await _mediator.Send(new HomeQuery(id));

        if (!homeResult.IsSuccess)
            (homeResult.Result as ErrorResult)!.ToActionResult();

        Home = homeResult.Value;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var result = await _mediator.Send(new DeleteHomeCommand(id));

        if (!result.IsSuccess)
            (result as ErrorResult)!.ToActionResult();

        return RedirectToPage("./Index");
    }
}
