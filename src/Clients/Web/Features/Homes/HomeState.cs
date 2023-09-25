using BlazorState;
using Spenses.Application.Models;

namespace Spenses.Client.Web.Features.Homes;

// todo: should this be called "root state" and be moved to higher folder?
public partial class HomeState : State<HomeState>
{
    public IEnumerable<Home> Homes { get; private set; }

    public Home SelectedHome { get; private set; }

    public override void Initialize()
    {
    }
}
