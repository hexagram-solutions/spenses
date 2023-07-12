using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Common.Results;
using Spenses.Application.Homes;
using Spenses.Domain.Models.Homes;
using Spenses.Web.Infrastructure;

namespace Spenses.Web.Pages.Homes;

public class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    public CreateModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public HomeProperties Home { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _mediator.Send(new CreateHomeCommand(Home));

        if (!result.IsSuccess)
            (result.Result as ErrorResult)!.ToActionResult();

        return RedirectToPage("./Index");
    }
}
