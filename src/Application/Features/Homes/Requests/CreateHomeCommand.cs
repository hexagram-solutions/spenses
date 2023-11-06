using AutoMapper;
using MediatR;
using Spenses.Application.Models.Homes;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Homes.Requests;

public record CreateHomeCommand(HomeProperties Props) : IRequest<Home>;

public class CreateHomeCommandHandler : IRequestHandler<CreateHomeCommand, Home>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateHomeCommandHandler(ApplicationDbContext db, IMapper mapper, ICurrentUserService currentUserService)
    {
        _db = db;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Home> Handle(CreateHomeCommand request, CancellationToken cancellationToken)
    {
        var home = _mapper.Map<DbModels.Home>(request.Props);

        var currentUser = _currentUserService.CurrentUser;

        home.Members.Add(new DbModels.Member
        {
            Name = currentUser.FindFirst(ApplicationClaimTypes.NickName)!.Value,
            ContactEmail = currentUser.FindFirst(ApplicationClaimTypes.Email)!.Value,
            DefaultSplitPercentage = 1m,
            UserId = currentUser.GetId()
        });

        var entry = await _db.Homes.AddAsync(home, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Home>(entry.Entity);
    }
}
