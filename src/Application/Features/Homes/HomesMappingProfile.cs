using AutoMapper;
using Spenses.Application.Models;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Homes;

public class HomesMappingProfile : Profile
{
    public HomesMappingProfile()
    {
        CreateMap<HomeProperties, DbModels.Home>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Members, opts => opts.Ignore())
            .ForMember(dest => dest.Expenses, opts => opts.Ignore());

        CreateMap<DbModels.Home, Home>();

        CreateMap<DbModels.Member, Member>();

        CreateMap<MemberProperties, DbModels.Member>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.Expenses, opts => opts.Ignore());

        CreateMap<ExpenseProperties, DbModels.Expense>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.IncurredByMemberId, opts => opts.Ignore())
            .ForMember(dest => dest.IncurredByMember, opts => opts.Ignore());

    }
}
