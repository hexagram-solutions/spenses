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

        ValidateProperties(props, expense.Home);

        mapper.Map(request.Props, expense);

        expense.PaidByMemberId = props.PaidByMemberId;
        expense.CategoryId = props.CategoryId;

        expense.ExpenseShares.Clear();

        foreach (var expenseShare in props.ExpenseShares)
        {
            expense.ExpenseShares.Add(new DbModels.ExpenseShare
            {
                OwedByMemberId = expenseShare.OwedByMemberId,
                OwedAmount = expenseShare.OwedAmount
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        var updatedExpense = await db.Expenses
            .ProjectTo<Expense>(mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == expense.Id, cancellationToken);

        return updatedExpense;
    }

    private static void ValidateProperties(ExpenseProperties props, DbModels.Home home)
    {
        if (home.Members.All(m => m.Id != props.PaidByMemberId))
            throw new InvalidRequestException($"Member {props.PaidByMemberId} is not a member of home {home.Id}");

        if (home.ExpenseCategories.All(ec => ec.Id != props.CategoryId))
            throw new InvalidRequestException($"Category {props.CategoryId} does not exist.");

        var invalidExpenseShareMemberIds = home.Members
            .Select(es => es.Id)
            .Except(props.ExpenseShares
                .Select(es => es.OwedByMemberId))
            .ToList();

        if (invalidExpenseShareMemberIds.Count != 0)
        {
            throw new InvalidRequestException($"Members {string.Join(", ", invalidExpenseShareMemberIds)} are not " +
                $"members of home {home.Id}");
        }
    }
}
