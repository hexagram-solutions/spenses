using Htmx;
using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Homes;
using Spenses.Domain.Models.Homes;

namespace Spenses.Web.Pages.Homes;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IEnumerable<Home> Homes { get; set; } = Array.Empty<Home>();

    public async Task OnGet()
    {
        var result = await _mediator.Send(new HomesQuery());

        Homes = result.Value;
    }


    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public async Task<IActionResult> OnGetRow()
    {
        var result = await _mediator.Send(new HomeQuery(Id));

        return Partial("_Row", result.Value);
    }

    public async Task<IActionResult> OnGetEdit()
    {
        var result = await _mediator.Send(new HomeQuery(Id));

        return Partial("_Edit", result.Value);
    }

    public async Task<IActionResult> OnPostUpdate([FromForm] HomeProperties props)
    {
        var result = await _mediator.Send(new HomeQuery(Id));

        if (!result.IsSuccess)
            return BadRequest();

        var updateResult = await _mediator.Send(new UpdateHomeCommand(Id, props));

        return Request.IsHtmx()
            ? Partial("_Row", updateResult.Value)
            : Redirect("Index");

    }
}
