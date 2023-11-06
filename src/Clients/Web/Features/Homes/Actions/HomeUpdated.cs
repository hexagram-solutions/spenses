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
            HomeState.HomeUpdating = true;

            var response = await _homes.PutHome(aAction.HomeId, aAction.Props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            HomeState.CurrentHome = response.Content;

            HomeState.HomeUpdating = false;
        }
    }
}
