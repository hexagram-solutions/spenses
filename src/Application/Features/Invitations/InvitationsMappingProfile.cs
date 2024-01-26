using AutoMapper;
using Spenses.Shared.Models.Invitations;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Invitations;

public class InvitationsMappingProfile : Profile
{
    public InvitationsMappingProfile()
    {
        CreateMap<DbModels.Invitation, Invitation>();
    }
}
