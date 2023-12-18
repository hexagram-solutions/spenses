using Fluxor;
using Spenses.Client.Http;
using Spenses.Web.Client.Infrastructure;
using Spenses.Web.Client.Store.Shared;

namespace Spenses.Web.Client.Store.Insights;

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
