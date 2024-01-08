using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Expenses.Requests;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/expenses")]
public class ExpensesController(IMediator mediator) : ControllerBase
{

    /// <summary>
    /// Create a new expense.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The expense properties.</param>
    /// <returns>The new expense.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Expense>> PostExpense(Guid homeId, ExpenseProperties props)
    {
        var expense = await mediator.Send(new CreateExpenseCommand(homeId, props));

        return CreatedAtAction(nameof(GetExpense), new { homeId, expenseId = expense.Id }, expense);
    }

    /// <summary>
    /// Query expenses with various parameters.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="query">The expense query parameters.</param>
    /// <returns>The sorted and filtered expenses.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<ExpenseDigest>>> GetExpenses(Guid homeId,
        [FromQuery] FilteredExpensesQuery query)
    {
        var expenses = await mediator.Send(new ExpensesQuery(homeId)
        {
            Skip = query.Skip,
            Take = query.Take,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            MinDate = query.MinDate,
            MaxDate = query.MaxDate,
            Tags = query.Tags,
            Categories = query.Categories,
        });

        return Ok(expenses);
    }

    /// <summary>
    /// Fetch an expense.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="expenseId">The expense identifier.</param>
    /// <returns>The expense.</returns>
    [HttpGet("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Expense>> GetExpense(Guid homeId, Guid expenseId)
    {
        var expense = await mediator.Send(new ExpenseQuery(homeId, expenseId));

        return Ok(expense);
    }

    /// <summary>
    /// Update an expense.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="expenseId">The expense identifier.</param>
    /// <param name="props">The new expense properties.</param>
    /// <returns>The updated expense.</returns>
    [HttpPut("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Expense>> PutExpense(Guid homeId, Guid expenseId, ExpenseProperties props)
    {
        var expenses = await mediator.Send(new UpdateExpenseCommand(homeId, expenseId, props));

        return Ok(expenses);
    }

    /// <summary>
    /// Delete an expense.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="expenseId">The expense identifier.</param>
    [HttpDelete("{expenseId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteExpense(Guid homeId, Guid expenseId)
    {
        await mediator.Send(new DeleteExpenseCommand(homeId, expenseId));

        return NoContent();
    }

    /// <summary>
    /// Fetch values used to filter expenses.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <returns>The expense filter values.</returns>
    [HttpGet("filters")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<ExpenseFilters>> Filters(Guid homeId)
    {
        var filters = await mediator.Send(new ExpenseFiltersQuery(homeId));

        return Ok(filters);
    }
}
