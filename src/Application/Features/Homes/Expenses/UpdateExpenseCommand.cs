using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Expenses;

public record UpdateExpenseCommand(Guid HomeId, Guid ExpenseId, ExpenseProperties Props) : IRequest<ServiceResult<Expense>>;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, ServiceResult<Expense>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateExpenseCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Expense>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, expenseId, props) = request;

        var expense = await _db.Expenses
            .Include(e => e.Home)
            .ThenInclude(h => h.Members)
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);

        if (expense is null)
            return new NotFoundErrorResult(expenseId);

        if (expense.Home.Members.All(m => m.Id != props.IncurredByMemberId))
        {
            return new InvalidRequestErrorResult(nameof(request.Props.IncurredByMemberId),
                $"Member {props.IncurredByMemberId} is not a member of home {homeId}");
        }

        _mapper.Map(request.Props, expense);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Expense>(expense);
    }
}
