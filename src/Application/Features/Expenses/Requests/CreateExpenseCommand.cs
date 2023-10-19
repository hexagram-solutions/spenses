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

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Expense>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateExpenseCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Expense> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await _db.Homes
            .Include(h => h.Members)
            .Include(h => h.ExpenseCategories)
            .FirstAsync(h => h.Id == homeId, cancellationToken);

        if (home.Members.All(m => m.Id != props.PaidByMemberId))
            throw new InvalidRequestException($"Member {props.PaidByMemberId} is not a member of home {homeId}");

        if (props.CategoryId.HasValue && home.ExpenseCategories.All(ec => ec.Id != props.CategoryId))
            throw new InvalidRequestException($"Category {props.CategoryId} does not exist.");

        var expense = _mapper.Map<DbModels.Expense>(props);

        expense.HomeId = homeId;
        expense.PaidByMemberId = props.PaidByMemberId;
        expense.CategoryId = props.CategoryId;

        foreach (var member in home.Members)
        {
            expense.ExpenseShares.Add(new DbModels.ExpenseShare
            {
                OwedByMemberId = member.Id,
                OwedPercentage = member.DefaultSplitPercentage,
                // Only add owing amounts for the other members; the member that paid the expense owes nothing.
                OwedAmount = member.Id != expense.PaidByMemberId
                    ? expense.Amount * member.DefaultSplitPercentage
                    : 0.00m
            });
        }

        home.Expenses.Add(expense);

        await _db.SaveChangesAsync(cancellationToken);

        var createdExpense = await _db.Expenses
            .ProjectTo<Expense>(_mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == expense.Id, cancellationToken);

        return createdExpense;
    }
}
