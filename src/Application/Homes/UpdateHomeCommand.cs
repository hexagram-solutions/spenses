using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Domain.Models.Homes;
using Spenses.Resources.Relational;

namespace Spenses.Application.Homes;

public record UpdateHomeCommand(Guid Id, HomeProperties Props) : IRequest<ServiceResult<Home>>;

public class UpdateHomeCommandHandler : IRequestHandler<UpdateHomeCommand, ServiceResult<Home>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateHomeCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Home>> Handle(UpdateHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes.FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (home is null)
            return new NotFoundErrorResult(request.Id);

        _mapper.Map(request.Props, home);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Home>(home);
    }
}
