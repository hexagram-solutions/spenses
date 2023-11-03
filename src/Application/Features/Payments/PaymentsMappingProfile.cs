using AutoMapper;
using Spenses.Application.Models.Payments;
using DbDigests = Spenses.Resources.Relational.DigestModels;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Payments;

public class PaymentsMappingProfile : Profile
{
    public PaymentsMappingProfile()
    {
        CreateMap<DbModels.Payment, Payment>();

        CreateMap<PaymentProperties, DbModels.Payment>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.PaidByMemberId, opts => opts.Ignore())
            .ForMember(dest => dest.PaidByMember, opts => opts.Ignore())
            .ForMember(dest => dest.PaidToMemberId, opts => opts.Ignore())
            .ForMember(dest => dest.PaidToMember, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());

        CreateMap<DbDigests.PaymentDigest, PaymentDigest>();
    }
}
