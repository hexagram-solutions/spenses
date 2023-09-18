using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spenses.Application.Features.Homes;
using Spenses.Application.Models;

namespace Spenses.Web.Pages.Homes;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IList<Home> Homes { get; set; } = Array.Empty<Home>();

    public async Task OnGet()
    {
        var result = await _mediator.Send(new HomesQuery());

        Homes = result.Value.ToList();
    }
}
