using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Expenses;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Expenses.Requests;

public record UpdateExpenseCommand(Guid HomeId, Guid ExpenseId, ExpenseProperties Props)
    : IAuthorizedRequest<Expense>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateExpenseCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<UpdateExpenseCommand, Expense>
{
    public async Task<Expense> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId, props) = request;

        var expense = await db.Expenses
            .Include(e => e.Home)
                .ThenInclude(h => h.Members)
            .Include(e => e.Home)
                .ThenInclude(h => h.ExpenseCategories)
            .Include(e => e.ExpenseShares)
            .Include(e => e.Tags)
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);

        if (expense is null)
            throw new ResourceNotFoundException(expenseId);

        if (expense.Home.Members.All(m => m.Id != props.PaidByMemberId))
            throw new InvalidRequestException($"Member {props.PaidByMemberId} is not a member of home {homeId}");

        if (expense.Home.ExpenseCategories.All(ec => ec.Id != props.CategoryId))
            throw new InvalidRequestException($"Category {props.CategoryId} does not exist.");

        mapper.Map(request.Props, expense);

        expense.PaidByMemberId = props.PaidByMemberId;
        expense.CategoryId = props.CategoryId;

        expense.ExpenseShares.Clear();

        foreach (var member in expense.Home.Members)
        {
            expense.ExpenseShares.Add(new DbModels.ExpenseShare
            {
                OwedByMemberId = member.Id,
                OwedPercentage = member.DefaultSplitPercentage,
                OwedAmount = expense.Amount * member.DefaultSplitPercentage
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        var updatedExpense = await db.Expenses
            .ProjectTo<Expense>(mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == expense.Id, cancellationToken);

        return updatedExpense;
    }
}
