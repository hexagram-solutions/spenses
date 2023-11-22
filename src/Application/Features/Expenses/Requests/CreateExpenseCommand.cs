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

public record CreateExpenseCommand(Guid HomeId, ExpenseProperties Props) : IAuthorizedRequest<Expense>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateExpenseCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<CreateExpenseCommand, Expense>
{
    public async Task<Expense> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await db.Homes
            .Include(h => h.Members)
            .Include(h => h.ExpenseCategories)
            .FirstAsync(h => h.Id == homeId, cancellationToken);

        ValidateProperties(props, home);

        var expense = mapper.Map<DbModels.Expense>(props);

        expense.HomeId = homeId;
        expense.PaidByMemberId = props.PaidByMemberId;
        expense.CategoryId = props.CategoryId;

        foreach (var expenseShare in props.ExpenseShares)
        {
            expense.ExpenseShares.Add(new DbModels.ExpenseShare
            {
                OwedByMemberId = expenseShare.OwedByMemberId,
                OwedAmount = expenseShare.OwedAmount,
                OwedPercentage = decimal.Divide(expenseShare.OwedAmount, expense.Amount)
            });
        }

        home.Expenses.Add(expense);

        await db.SaveChangesAsync(cancellationToken);

        var createdExpense = await db.Expenses
            .ProjectTo<Expense>(mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == expense.Id, cancellationToken);

        return createdExpense;
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
