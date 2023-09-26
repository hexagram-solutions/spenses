using BlazorState;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Homes;

public partial class HomeState
{
    public record HomeSelected(Guid HomeId) : IAction;

    public class HomeSelectedHandler : ActionHandler<HomeSelected>
    {
        private readonly IHomesApi _homes;
        private readonly NavigationManager _navigationManager;

        public HomeSelectedHandler(IStore store, IHomesApi homes, NavigationManager navigationManager)
            : base(store)
        {
            _homes = homes;
            _navigationManager = navigationManager;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomeSelected aAction, CancellationToken aCancellationToken)
        {
            if (HomeState.CurrentHome?.Id == aAction.HomeId)
                return;

            HomeState.HomeRequesting = true;

            var home = await _homes.GetHome(aAction.HomeId);

            HomeState.CurrentHome = home;

            HomeState.HomeRequesting = false;

            //_navigationManager.NavigateTo($"/homes/{home.Id}/dashboard");
        }
    }
}
