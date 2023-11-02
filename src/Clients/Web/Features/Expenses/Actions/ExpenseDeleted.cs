using BlazorState;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Expenses;

public partial class ExpensesState
{
    public record ExpenseDeleted(Guid HomeId, Guid ExpenseId) : IAction;

    public class ExpenseDeletedHandler : ActionHandler<ExpenseDeleted>
    {
        private readonly IExpensesApi _expenses;

        public ExpenseDeletedHandler(IStore aStore, IExpensesApi expenses)
            : base(aStore)
        {
            _expenses = expenses;
        }

        private ExpensesState ExpensesState => Store.GetState<ExpensesState>();

        public override async Task Handle(ExpenseDeleted aAction, CancellationToken aCancellationToken)
        {
            ExpensesState.ExpenseDeleting = true;

            var (homeId, expenseId) = aAction;

            var response = await _expenses.DeleteExpense(homeId, expenseId);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            ExpensesState.ExpenseDeleting = false;
        }
    }
}
