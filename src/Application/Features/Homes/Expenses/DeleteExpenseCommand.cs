using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Expenses;

public record DeleteExpenseCommand(Guid HomeId, Guid ExpenseId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand>
{
    private readonly ApplicationDbContext _db;

    public DeleteExpenseCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var expense = await _db.Expenses
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);

        if (expense is null)
            throw new ResourceNotFoundException(expenseId);

        _db.Expenses.Remove(expense);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
