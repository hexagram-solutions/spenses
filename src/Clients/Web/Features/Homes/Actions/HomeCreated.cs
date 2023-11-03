using BlazorState;
using MediatR;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Homes;

public partial class HomeState
{
    public record HomeCreated(HomeProperties Props) : IAction;

    public class HomeCreatedHandler : ActionHandler<HomeCreated>
    {
        private readonly IHomesApi _homes;
        private readonly NavigationManager _navigationManager;
        private readonly IMediator _mediator;

        public HomeCreatedHandler(IStore store, IHomesApi homes, NavigationManager navigationManager,
            IMediator mediator)
            : base(store)
        {
            _homes = homes;
            _navigationManager = navigationManager;
            _mediator = mediator;
        }

        private HomeState HomeState => Store.GetState<HomeState>();

        public override async Task Handle(HomeCreated aAction, CancellationToken aCancellationToken)
        {
            HomeState.HomeCreating = true;

            var homeResponse = await _homes.PostHome(aAction.Props);

            if (!homeResponse.IsSuccessStatusCode)
                throw new NotImplementedException();

            HomeState.HomeCreating = false;

            await _mediator.Send(new HomesRequested(), aCancellationToken);

            _navigationManager.NavigateTo($"homes/{homeResponse.Content!.Id}/dashboard");
        }
    }
}
