using BlazorState;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Expenses;

public partial class ExpensesState
{
    public record ExpensesRequested(Guid HomeId, FilteredExpensesQuery Query) : IAction;

    public class ExpensesRequestedHandler : ActionHandler<ExpensesRequested>
    {
        private readonly IExpensesApi _expenses;

        public ExpensesRequestedHandler(IStore aStore, IExpensesApi expenses)
            : base(aStore)
        {
            _expenses = expenses;
        }

        private ExpensesState ExpenseState => Store.GetState<ExpensesState>();

        public override async Task Handle(ExpensesRequested aAction, CancellationToken aCancellationToken)
        {
            ExpenseState.ExpensesRequesting = true;

            var expensesResponse = (await _expenses.GetExpenses(aAction.HomeId, aAction.Query));

            if (!expensesResponse.IsSuccessStatusCode)
                throw new NotImplementedException();

            ExpenseState.Expenses = expensesResponse.Content;

            ExpenseState.ExpensesRequesting = false;
        }
    }
}
