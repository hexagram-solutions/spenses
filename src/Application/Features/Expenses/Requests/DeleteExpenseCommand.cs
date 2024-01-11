using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Expenses.Requests;

public record DeleteExpenseCommand(Guid HomeId, Guid ExpenseId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteExpenseCommandHandler(ApplicationDbContext db) : IRequestHandler<DeleteExpenseCommand>
{
    public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var expense = await db.Expenses
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);

        if (expense is null)
            throw new ResourceNotFoundException(expenseId);

        db.Expenses.Remove(expense);

        await db.SaveChangesAsync(cancellationToken);
    }
}
