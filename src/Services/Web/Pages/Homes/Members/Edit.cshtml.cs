using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Common.Results;
using Spenses.Application.Homes.Members;
using Spenses.Application.Models;
using Spenses.Web.Infrastructure;

namespace Spenses.Web.Pages.Homes.Members;

public class EditModel : PageModel
{
    private readonly IMediator _mediator;

    public EditModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public MemberProperties Member { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id) // todo: why is id nullable?
    {
        var memberResult = await _mediator.Send(new MemberQuery(id));

        if (!memberResult.IsSuccess)
            (memberResult.Result as ErrorResult)!.ToActionResult();

        Member = memberResult.Value;

        return Page();
    }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync([FromRoute] Guid homeId, [FromRoute] Guid id)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _mediator.Send(new UpdateMemberCommand(id, Member));

        if (!result.IsSuccess)
            (result.Result as ErrorResult)!.ToActionResult();

        return RedirectToPage("/Homes/Details", new { id = homeId });
    }
}
