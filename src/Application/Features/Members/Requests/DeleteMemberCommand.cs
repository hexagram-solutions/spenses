using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record DeleteMemberCommand(Guid HomeId, Guid MemberId) : IAuthorizedRequest<DeletionResult<Member>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, DeletionResult<Member>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public DeleteMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<DeletionResult<Member>> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await _db.Members
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == memberId, cancellationToken);

        if (member is null)
            throw new ResourceNotFoundException(memberId);

        _db.Members.Remove(member);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);

            return new DeletionResult<Member>(DeletionType.Deleted, _mapper.Map<Member>(member));
        }
        catch (DbUpdateException ex)
            when (ex.Entries.SingleOrDefault(e => (e.Entity as DbModels.Member)?.Id == memberId) is not null)
        {
            // The member has associated entities and can't be deleted, so we'll deactivate the member instead.
            member.IsActive = false;

            // "Undo" the prior removal
            _db.Members.Entry(member).State = EntityState.Modified;

            await _db.SaveChangesAsync(cancellationToken);

            return new DeletionResult<Member>(DeletionType.Deactivated, _mapper.Map<Member>(member));
        }
    }
}
