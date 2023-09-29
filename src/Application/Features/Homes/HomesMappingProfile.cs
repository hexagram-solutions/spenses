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
            .ForMember(dest => dest.Expenses, opts => opts.Ignore())
            .ForMember(dest => dest.Credits, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());

        CreateMap<DbModels.Home, Home>();

        CreateMap<DbModels.Member, Member>();

        CreateMap<MemberProperties, DbModels.Member>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.User, opts => opts.Ignore())
            .ForMember(dest => dest.UserId, opts => opts.Ignore())
            .ForMember(dest => dest.Expenses, opts => opts.Ignore())
            .ForMember(dest => dest.Credits, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());

        CreateMap<DbModels.Expense, Expense>();

        CreateMap<ExpenseProperties, DbModels.Expense>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.IncurredByMemberId, opts => opts.Ignore())
            .ForMember(dest => dest.IncurredByMember, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());

        CreateMap<DbModels.Credit, Credit>();

        CreateMap<CreditProperties, DbModels.Credit>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.PaidByMemberId, opts => opts.Ignore())
            .ForMember(dest => dest.PaidByMember, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());
    }
}
