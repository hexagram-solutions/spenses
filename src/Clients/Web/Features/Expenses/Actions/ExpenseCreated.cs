using BlazorState;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Expenses;

public partial class ExpensesState
{
    public record ExpenseCreated(Guid HomeId, ExpenseProperties Props) : IAction;

    public class ExpenseCreatedHandler : ActionHandler<ExpenseCreated>
    {
        private readonly IExpensesApi _expenses;

        public ExpenseCreatedHandler(IStore aStore, IExpensesApi expenses)
            : base(aStore)
        {
            _expenses = expenses;
        }

        private ExpensesState ExpensesState => Store.GetState<ExpensesState>();

        public override async Task Handle(ExpenseCreated aAction, CancellationToken aCancellationToken)
        {
            ExpensesState.ExpenseCreating = true;

            var (homeId, props) = aAction;

            var response = await _expenses.PostExpense(homeId, props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            ExpensesState.ExpenseCreating = false;
        }
    }
}
