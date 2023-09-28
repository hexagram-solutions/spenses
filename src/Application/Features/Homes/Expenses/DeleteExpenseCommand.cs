using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Authorization;
using Spenses.Application.Common.Results;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Expenses;

public record DeleteExpenseCommand(Guid HomeId, Guid ExpenseId) : IAuthorizedRequest<ServiceResult>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult OnUnauthorized() => new UnauthorizedErrorResult();
}

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, ServiceResult>
{
    private readonly ApplicationDbContext _db;

    public DeleteExpenseCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ServiceResult> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var expense = await _db.Expenses
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);

        if (expense is null)
            return new NotFoundErrorResult(expenseId);

        _db.Expenses.Remove(expense);

        await _db.SaveChangesAsync(cancellationToken);

        return new SuccessResult();
    }
}
