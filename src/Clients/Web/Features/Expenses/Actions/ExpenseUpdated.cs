using BlazorState;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Expenses;

public partial class ExpensesState
{
    public record ExpenseUpdated(Guid HomeId, Guid ExpenseId, ExpenseProperties Props) : IAction;

    public class ExpenseUpdatedHandler : ActionHandler<ExpenseUpdated>
    {
        private readonly IExpensesApi _expenses;

        public ExpenseUpdatedHandler(IStore aStore, IExpensesApi expenses)
            : base(aStore)
        {
            _expenses = expenses;
        }

        private ExpensesState ExpensesState => Store.GetState<ExpensesState>();

        public override async Task Handle(ExpenseUpdated aAction, CancellationToken aCancellationToken)
        {
            ExpensesState.ExpenseUpdating = true;

            var (homeId, expenseId, props) = aAction;

            var response = await _expenses.PutExpense(homeId, expenseId, props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            ExpensesState.ExpenseUpdating = false;
        }
    }
}
