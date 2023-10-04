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
            .FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home is null)
            throw new ResourceNotFoundException(homeId);

        if (home.Members.All(m => m.Id != props.IncurredByMemberId))
            throw new InvalidRequestException($"Member {props.IncurredByMemberId} is not a member of home {homeId}");

        var expense = _mapper.Map<DbModels.Expense>(props);

        expense.HomeId = homeId;
        expense.IncurredByMemberId = props.IncurredByMemberId;

        home.Expenses.Add(expense);

        await _db.SaveChangesAsync(cancellationToken);

        var createdExpense = await _db.Expenses
            .ProjectTo<Expense>(_mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == expense.Id, cancellationToken);

        return createdExpense;
    }
}
