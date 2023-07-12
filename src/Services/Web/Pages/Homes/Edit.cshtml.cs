using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Common.Results;
using Spenses.Application.Homes;
using Spenses.Domain.Models.Homes;
using Spenses.Web.Infrastructure;

namespace Spenses.Web.Pages.Homes;

public class EditModel : PageModel
{
    private readonly IMediator _mediator;

    public EditModel(IMediator mediator)
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

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _mediator.Send(new UpdateHomeCommand(Home.Id, Home));

        if (!result.IsSuccess)
            (result.Result as ErrorResult)!.ToActionResult();

        return RedirectToPage("./Index");
    }
}
