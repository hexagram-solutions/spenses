using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Homes.Expenses;

public record CreateExpenseCommand(Guid HomeId, ExpenseProperties Props) : IRequest<ServiceResult<Expense>>;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ServiceResult<Expense>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateExpenseCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Expense>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await _db.Homes
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home is null)
            return new NotFoundErrorResult(homeId);

        if (home.Members.All(m => m.Id != props.IncurredByMemberId))
        {
            return new InvalidRequestErrorResult(nameof(request.Props.IncurredByMemberId),
                $"Member {props.IncurredByMemberId} is not a member of home {homeId}");
        }

        var expense = _mapper.Map<DbModels.Expense>(props);

        home.Expenses.Add(expense);

        await _db.SaveChangesAsync(cancellationToken);

        var entity = _db.Entry(expense).Entity;

        return _mapper.Map<Expense>(entity);
    }
}
