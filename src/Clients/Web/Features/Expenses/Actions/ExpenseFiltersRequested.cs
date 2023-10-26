using BlazorState;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Expenses;

public partial class ExpensesState
{
    public record ExpenseFiltersRequested(Guid HomeId) : IAction;

    public class ExpensesFiltersRequestedHandler : ActionHandler<ExpenseFiltersRequested>
    {
        private readonly IExpensesApi _expenses;

        public ExpensesFiltersRequestedHandler(IStore aStore, IExpensesApi expenses)
            : base(aStore)
        {
            _expenses = expenses;
        }

        private ExpensesState ExpenseState => Store.GetState<ExpensesState>();

        public override async Task Handle(ExpenseFiltersRequested aAction, CancellationToken aCancellationToken)
        {
            ExpenseState.ExpenseFiltersRequesting = true;

            var expensesResponse = await _expenses.GetExpenseFilters(aAction.HomeId);

            if (!expensesResponse.IsSuccessStatusCode)
                throw new NotImplementedException();

            ExpenseState.ExpenseFilters = expensesResponse.Content;

            ExpenseState.ExpenseFiltersRequesting = false;
        }
    }
}
