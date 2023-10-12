using BlazorState;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Homes;

public partial class HomeState
{
    public record HomeSelected(Guid HomeId) : IAction;

    public class HomeSelectedHandler : ActionHandler<HomeSelected>
    {
        private readonly IHomesApi _homes;

        public HomeSelectedHandler(IStore store, IHomesApi homes)
            : base(store)
        {
            _homes = homes;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomeSelected aAction, CancellationToken aCancellationToken)
        {
            if (HomeState.CurrentHome?.Id == aAction.HomeId)
                return;

            HomeState.HomeRequesting = true;

            var home = await _homes.GetHome(aAction.HomeId);

            HomeState.CurrentHome = home.Content;

            HomeState.HomeRequesting = false;
        }
    }
}
