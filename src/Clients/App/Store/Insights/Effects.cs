using Fluxor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.Insights;

public class Effects(IInsightsApi insights)
{
    [EffectMethod]
    public async Task HandleExpensesOverTimeRequested(ExpensesOverTimeRequestedAction action, IDispatcher dispatcher)
    {
        var response = await insights.GetExpensesOverTime(action.HomeId, action.Grouping);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new ExpensesOverTimeRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new ExpensesOverTimeReceivedAction(response.Content!.ToArray()));
    }
}
