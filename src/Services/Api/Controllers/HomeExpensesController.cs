using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Expenses;
using Spenses.Application.Models;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("/homes/{homeId:guid}/expenses")]
public class HomeExpensesController : ApiControllerBase
{
    public HomeExpensesController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Expense>> PostExpense(Guid homeId, ExpenseProperties props)
    {
        return await GetCommandResult<Expense, CreateExpenseCommand>(
            new CreateExpenseCommand(homeId, props),
            x => CreatedAtAction(nameof(GetExpense), new { homeId, expenseId = x.Id }, x));
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses(Guid homeId)
    {
        return await GetCommandResult<IEnumerable<Expense>, ExpensesQuery>(new ExpensesQuery(homeId), Ok);
    }

    [HttpGet("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Expense>> GetExpense(Guid homeId, Guid expenseId)
    {
        return await GetCommandResult<Expense, ExpenseQuery>(new ExpenseQuery(homeId, expenseId), Ok);
    }

    [HttpPut("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Expense>> PutExpense(Guid homeId, Guid expenseId, ExpenseProperties props)
    {
        return await GetCommandResult<Expense, UpdateExpenseCommand>(
            new UpdateExpenseCommand(homeId, expenseId, props), Ok);
    }

    [HttpDelete("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public Task<ActionResult> DeleteMember(Guid homeId, Guid expenseId)
    {
        return GetCommandResult(new DeleteExpenseCommand(homeId, expenseId), NoContent);
    }
}
