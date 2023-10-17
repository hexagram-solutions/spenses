using BlazorState;
using Spenses.Application.Models.Homes;

namespace Spenses.Client.Web.Features.Homes;

// todo: should this be called "root state" and be moved to higher folder?
public partial class HomeState : State<HomeState>
{
    public Home? CurrentHome { get; private set; }

    public IEnumerable<Home>? Homes { get; private set; }

    public bool HomesRequesting { get; private set; }

    public bool HomeRequesting { get; private set; }

    public bool HomeCreating { get; private set; }

    public override void Initialize()
    {
        Homes = new List<Home>();
    }
}
