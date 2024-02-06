using Fluxor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.Dashboard;

public class Effects(IHomesApi homes, IState<DashboardState> state)
{
    [EffectMethod]
    public Task HandleDashboardPeriodChanged(DashboardPeriodChangedAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new BalanceBreakdownRequestedAction(action.HomeId));

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleBalanceBreakdownRequested(BalanceBreakdownRequestedAction action, IDispatcher dispatcher)
    {
        var response = await homes.GetBalanceBreakdown(action.HomeId, state.Value.PeriodStart, state.Value.PeriodEnd);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new BalanceBreakdownRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new BalanceBreakdownReceivedAction(response.Content!));
    }
}
