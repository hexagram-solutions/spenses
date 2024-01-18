using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.ExpenseCategories.Requests;

public record DeleteExpenseCategoryCommand(Guid HomeId, Guid ExpenseCategoryId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteExpenseCategoryCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DeleteExpenseCategoryCommand>
{
    public async Task Handle(DeleteExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId) = request;

        var category = await db.ExpenseCategories
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);

        if (category is null)
            throw new ResourceNotFoundException(expenseId);

        if (category.IsDefault)
            throw new InvalidRequestException("A home's default expense category cannot be deleted.");

        db.ExpenseCategories.Remove(category);

        await db.SaveChangesAsync(cancellationToken);
    }
}
