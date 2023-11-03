using BlazorState;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Expenses;

public partial class ExpensesState
{
    public record ExpenseSelected(Guid HomeId, Guid ExpenseId) : IAction;

    public class ExpenseSelectedHandler : ActionHandler<ExpenseSelected>
    {
        private readonly IExpensesApi _expenses;

        public ExpenseSelectedHandler(IStore aStore, IExpensesApi expenses)
            : base(aStore)
        {
            _expenses = expenses;
        }

        private ExpensesState ExpensesState => Store.GetState<ExpensesState>();

        public override async Task Handle(ExpenseSelected aAction, CancellationToken aCancellationToken)
        {
            ExpensesState.ExpenseRequesting = true;

            var (homeId, expenseId) = aAction;

            var response = await _expenses.GetExpense(homeId, expenseId);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            ExpensesState.CurrentExpense = response.Content;

            ExpensesState.ExpenseRequesting = false;
        }
    }
}
