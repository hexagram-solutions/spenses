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

            var (homeId, props) = aAction;

            var response = await _homes.PutHome(homeId, props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            var updatedHome = response.Content;

            HomeState.CurrentHome = updatedHome;

            // Update home in homes collection so that any references to this home in lists or headers (e.g.: the nav
            // menu) are update.
            var homesItem = HomeState.Homes.Single(x => x.Id == homeId);

            homesItem.Name = updatedHome!.Name;

            HomeState.HomeUpdating = false;
        }
    }
}
