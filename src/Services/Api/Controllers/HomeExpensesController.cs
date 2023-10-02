using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Expenses;
using Spenses.Application.Models;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("/homes/{homeId:guid}/expenses")]
public class HomeExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Expense>> PostExpense(Guid homeId, ExpenseProperties props)
    {
        var expense = await _mediator.Send(new CreateExpenseCommand(homeId, props));

        return CreatedAtAction(nameof(GetExpense), new { homeId, expenseId = expense.Id }, expense);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses(Guid homeId)
    {
        var expenses = await _mediator.Send(new ExpensesQuery(homeId));

        return Ok(expenses);
    }

    [HttpGet("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Expense>> GetExpense(Guid homeId, Guid expenseId)
    {
        var expense = await _mediator.Send(new ExpenseQuery(homeId, expenseId));

        return Ok(expense);
    }

    [HttpPut("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Expense>> PutExpense(Guid homeId, Guid expenseId, ExpenseProperties props)
    {
        var expenses = await _mediator.Send(new UpdateExpenseCommand(homeId, expenseId, props));

        return Ok(expenses);
    }

    [HttpDelete("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteMember(Guid homeId, Guid expenseId)
    {
        await _mediator.Send(new DeleteExpenseCommand(homeId, expenseId));

        return NoContent();
    }
}
