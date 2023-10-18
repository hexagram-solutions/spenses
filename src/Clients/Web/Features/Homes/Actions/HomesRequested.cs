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

        public HomesRequestedHandler(IStore aStore, IHomesApi homes)
            : base(aStore)
        {
            _homes = homes;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomesRequested aAction, CancellationToken aCancellationToken)
        {
            HomeState.HomesRequesting = true;

            var homes = (await _homes.GetHomes()).Content!.ToArray();

            HomeState.Homes = homes;

            HomeState.HomesRequesting = false;
        }
    }
}
