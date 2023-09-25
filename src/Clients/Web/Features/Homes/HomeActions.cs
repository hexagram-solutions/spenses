using System;
using BlazorState;
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

        public HomesRequestedHandler(IStore aStore, IHomesApi homes) : base(aStore)
        {
            _homes = homes;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomesRequested aAction, CancellationToken aCancellationToken)
        {
            var homes = await _homes.GetHomes();

            HomeState.Homes = homes;
        }
    }

    public class HomeRequested : IAction
    {
        public Guid HomeId { get; set; }
    }

    public class HomeRequestedHandler : ActionHandler<HomeState.HomeRequested>
    {
        private readonly IHomesApi _homes;

        public HomeRequestedHandler(IStore store, IHomesApi homes) : base(store)
        {
            _homes = homes;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomeRequested action, CancellationToken cancellationToken)
        {
            var home = await _homes.GetHome(action.HomeId);

            HomeState.SelectedHome = home;
        }
    }
}


