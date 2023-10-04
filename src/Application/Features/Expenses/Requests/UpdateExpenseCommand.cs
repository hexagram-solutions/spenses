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

namespace Spenses.Application.Features.Expenses.Requests;

public record UpdateExpenseCommand(Guid HomeId, Guid ExpenseId, ExpenseProperties Props)
    : IAuthorizedRequest<Expense>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Expense>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateExpenseCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Expense> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId, props) = request;

        var expense = await _db.Expenses
            .Include(e => e.Home)
                .ThenInclude(h => h.Members)
            .Include(e => e.Tags)
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);

        if (expense is null)
            throw new ResourceNotFoundException(expenseId);

        if (expense.Home.Members.All(m => m.Id != props.IncurredByMemberId))
            throw new InvalidRequestException($"Member {props.IncurredByMemberId} is not a member of home {homeId}");

        _mapper.Map(request.Props, expense);

        expense.IncurredByMemberId = props.IncurredByMemberId;

        await _db.SaveChangesAsync(cancellationToken);

        var updatedExpense = await _db.Expenses
            .ProjectTo<Expense>(_mapper.ConfigurationProvider)
            .FirstAsync(e => e.Id == expense.Id, cancellationToken);

        return updatedExpense;
    }
}
