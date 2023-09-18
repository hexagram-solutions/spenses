using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Common.Results;
using Spenses.Application.Homes.Members;
using Spenses.Application.Models;
using Spenses.Web.Infrastructure;

namespace Spenses.Web.Pages.Homes.Members;

public class AddModel : PageModel
{
    private readonly IMediator _mediator;

    public AddModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public MemberProperties Member { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync([FromRoute] Guid homeId)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _mediator.Send(new AddMemberToHomeCommand(homeId, Member));

        if (!result.IsSuccess)
            (result.Result as ErrorResult)!.ToActionResult();

        return RedirectToPage("/Homes/Details", new {id = homeId });
    }
}
