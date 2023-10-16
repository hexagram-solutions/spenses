using BlazorState;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Homes;

public partial class HomeState
{
    public class HomesRequested : IAction
    {
    }

    public class HomesRequestedHandler : ActionHandler<HomesRequested>
    {
        private readonly IHomesApi _homes;
        private readonly NavigationManager _navigationManager;

        public HomesRequestedHandler(IStore aStore, IHomesApi homes, NavigationManager navigationManager)
            : base(aStore)
        {
            _homes = homes;
            _navigationManager = navigationManager;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomesRequested aAction, CancellationToken aCancellationToken)
        {
            HomeState.HomesRequesting = true;

            var homes = (await _homes.GetHomes()).Content!.ToArray();

            HomeState.Homes = homes;

            HomeState.HomesRequesting = false;

            if (HomeState.CurrentHome is null)
                _navigationManager.NavigateTo($"/homes/{homes.First().Id}/dashboard");
        }
    }
}
