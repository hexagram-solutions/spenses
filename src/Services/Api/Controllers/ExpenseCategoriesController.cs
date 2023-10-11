using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.ExpenseCategories.Requests;
using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/expense-categories")]
public class ExpenseCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpenseCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new expense category.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The expense category properties.</param>
    /// <returns>The new expense category.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<ExpenseCategory>> PostExpenseCategory(Guid homeId, ExpenseCategoryProperties props)
    {
        var category = await _mediator.Send(new CreateExpenseCategoryCommand(homeId, props));

        return CreatedAtAction(nameof(GetExpenseCategory), new { homeId, expenseCategoryId = category.Id }, category);
    }

    /// <summary>
    /// Fetch all expense categories for a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <returns>The expense categories</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetExpenseCategories(Guid homeId)
    {
        var categories = await _mediator.Send(new ExpenseCategoriesQuery(homeId));

        return Ok(categories);
    }

    /// <summary>
    /// Fetch an expense category.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="expenseCategoryId">The expense category identifier.</param>
    /// <returns>The expense.</returns>
    [HttpGet("{expenseCategoryId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<ExpenseCategory>> GetExpenseCategory(Guid homeId, Guid expenseCategoryId)
    {
        var category = await _mediator.Send(new ExpenseCategoryQuery(homeId, expenseCategoryId));

        return Ok(category);
    }

    /// <summary>
    /// Update an expense category.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="expenseCategoryId">The expense category identifier.</param>
    /// <param name="props">The new expense category properties.</param>
    /// <returns>The updated expense category.</returns>
    [HttpPut("{expenseCategoryId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<ExpenseCategory>> PutExpenseCategory(Guid homeId, Guid expenseCategoryId,
        ExpenseCategoryProperties props)
    {
        var category = await _mediator.Send(new UpdateExpenseCategoryCommand(homeId, expenseCategoryId, props));

        return Ok(category);
    }

    /// <summary>
    /// Delete an expense category.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="expenseCategoryId">The expense category identifier.</param>
    [HttpDelete("{expenseCategoryId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteExpenseCategory(Guid homeId, Guid expenseCategoryId)
    {
        await _mediator.Send(new DeleteExpenseCategoryCommand(homeId, expenseCategoryId));

        return NoContent();
    }
}