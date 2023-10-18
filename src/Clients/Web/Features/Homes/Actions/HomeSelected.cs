using BlazorState;
using MediatR;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Homes;

public partial class HomeState
{
    public record HomeSelected(Guid HomeId) : IAction;

    public class HomeSelectedHandler : ActionHandler<HomeSelected>
    {
        private readonly IHomesApi _homes;
        private readonly IMediator _mediator;

        public HomeSelectedHandler(IStore store, IHomesApi homes, IMediator mediator)
            : base(store)
        {
            _homes = homes;
            _mediator = mediator;
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
