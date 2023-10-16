using BlazorState;
using Spenses.Application.Models.Homes;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Homes;

public partial class HomeState
{
    public record HomeUpdated(Guid HomeId, HomeProperties Props) : IAction;

    public class HomeUpdatedHandler : ActionHandler<HomeUpdated>
    {
        private readonly IHomesApi _homes;

        public HomeUpdatedHandler(IStore aStore, IHomesApi homes)
            : base(aStore)
        {
            _homes = homes;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomeUpdated aAction, CancellationToken aCancellationToken)
        {
            var updated = await _homes.PutHome(aAction.HomeId, aAction.Props);

            HomeState.CurrentHome = updated.Content;
        }
    }
}
