using BlazorState;
using MediatR;
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
        private readonly IMediator _mediator;

        public HomesRequestedHandler(IStore aStore, IHomesApi homes, IMediator mediator) : base(aStore)
        {
            _homes = homes;
            _mediator = mediator;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomesRequested aAction, CancellationToken aCancellationToken)
        {
            HomeState.HomesRequesting = true;

            var homes = (await _homes.GetHomes()).Content!.ToArray();

            HomeState.Homes = homes;

            HomeState.HomesRequesting = false;

            // TODO: Some kind of saga
            if (HomeState.CurrentHome is null)
                await _mediator.Send(new HomeSelected(homes.First().Id), aCancellationToken);
        }
    }
}
