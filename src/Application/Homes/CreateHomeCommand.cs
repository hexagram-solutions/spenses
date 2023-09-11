using AutoMapper;
using MediatR;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Homes;

public record CreateHomeCommand(HomeProperties Props) : IRequest<ServiceResult<Home>>;

public class CreateHomeCommandHandler : IRequestHandler<CreateHomeCommand, ServiceResult<Home>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateHomeCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Home>> Handle(CreateHomeCommand request, CancellationToken cancellationToken)
    {
        var home = _mapper.Map<DbModels.Home>(request.Props);

        var entry = await _db.Homes.AddAsync(home, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Home>(entry.Entity);
    }
}
