using AutoMapper;
using Spenses.Shared.Models.Invitations;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Invitations;

public class InvitationsMappingProfile : Profile
{
    public InvitationsMappingProfile()
    {
        CreateMap<DbModels.Invitation, Invitation>();

        CreateMap<InvitationProperties, DbModels.Invitation>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Status, opts => opts.Ignore())
            .ForMember(dest => dest.Member, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());
    }
}
