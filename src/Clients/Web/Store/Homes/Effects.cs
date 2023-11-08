using Fluxor;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Store.Homes;

public class Effects
{
    private readonly IHomesApi _homes;

    public Effects(IHomesApi homes)
    {
        _homes = homes;
    }

    [EffectMethod]
    public async Task HandleHomesRequested(HomesRequestedAction _, IDispatcher dispatcher)
    {
        var homesResponse = await _homes.GetHomes();

        if (!homesResponse.IsSuccessStatusCode)
            dispatcher.Dispatch(new HomesRequestFailedAction(homesResponse.Error.ReasonPhrase!));

        dispatcher.Dispatch(new HomesReceivedAction(homesResponse.Content!));
    }
}
