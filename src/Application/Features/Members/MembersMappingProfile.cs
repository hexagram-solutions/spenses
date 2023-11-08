using AutoMapper;
using Spenses.Application.Models.Members;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members;

public class MembersMappingProfile : Profile
{
    public MembersMappingProfile()
    {
        CreateMap<DbModels.Member, Member>();

        CreateMap<MemberProperties, DbModels.Member>()
            // The only way IsActive can be set to false is by deleting/deactivating the member.
            .ForMember(dest => dest.IsActive, opts => opts.MapFrom(x => true))
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.User, opts => opts.Ignore())
            .ForMember(dest => dest.UserId, opts => opts.Ignore())
            .ForMember(dest => dest.ExpensesPaid, opts => opts.Ignore())
            .ForMember(dest => dest.ExpenseShares, opts => opts.Ignore())
            .ForMember(dest => dest.PaymentsPaid, opts => opts.Ignore())
            .ForMember(dest => dest.PaymentsReceived, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());
    }
}
