using AutoMapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Features.Users;

public record SyncCurrentUserCommand : IRequest<ServiceResult<User>>;

public class SyncCurrentUserCommandHandler : IRequestHandler<SyncCurrentUserCommand, ServiceResult<User>>
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public SyncCurrentUserCommandHandler(ApplicationDbContext db, ICurrentUserService currentUserService, IMapper mapper)
    {
        _db = db;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ServiceResult<User>> Handle(SyncCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.CurrentUser;
        var currentUserId = currentUser.GetId();

        var userIdentity = await _db.Users.FirstOrDefaultAsync(c => c.Id == currentUserId, cancellationToken);

        if (userIdentity is null)
        {
            userIdentity = _mapper.Map<UserIdentity>(currentUser);

            var currentUserEntry = await _db.Users.AddAsync(userIdentity, cancellationToken);

            userIdentity = currentUserEntry.Entity;
        }
        else
        {
            _mapper.Map(currentUser, userIdentity);
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        // Handle the scenario where the current user was added moments ago on a parallel request. In this case, there
        // is no need to update the user since they were just created, so do nothing.
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 })
        {
            // Error number 2601 represents a duplicate key violation: "Cannot insert duplicate key row in object '%.*ls'
            // with unique index '%.*ls'."
            // https://docs.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-events-and-errors#errors-2000-to-2999
        }

        return _mapper.Map<User>(userIdentity);
    }
}
