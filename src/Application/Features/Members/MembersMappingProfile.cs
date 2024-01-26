using AutoMapper;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Utilities;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members;

public class MembersMappingProfile : Profile
{
    public MembersMappingProfile()
    {
        CreateMap<DbModels.Member, Member>()
            .ForMember(dest => dest.AvatarUrl, opts => opts.MapFrom(m => m.User != null ? m.User!.AvatarUrl : null))
            .AfterMap((_, member) =>
            {
                if (!string.IsNullOrEmpty(member.AvatarUrl))
                    return;

                member.AvatarUrl = AvatarHelper.GetGravatarUri(string.Empty, 80, true, GravatarDefaultType.Mp)
                    .ToString();
            });

        CreateMap<MemberProperties, DbModels.Member>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Status, opts => opts.Ignore())
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
