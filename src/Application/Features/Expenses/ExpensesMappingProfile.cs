using AutoMapper;
using Spenses.Shared.Models.Expenses;
using DbDigests = Spenses.Resources.Relational.DigestModels;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Expenses;

public class ExpensesMappingProfile : Profile
{
    public ExpensesMappingProfile()
    {
        CreateMap<DbModels.Expense, Expense>()
            .ForMember(dest => dest.Tags, opts => opts.MapFrom(src => src.Tags.Select(t => t.Name)));

        CreateMap<ExpenseProperties, DbModels.Expense>()
            .ForMember(dest => dest.Tags,
                opts => opts.MapFrom(src => src.Tags.Select(t => new DbModels.ExpenseTag { Name = t })))
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.Category, opts => opts.Ignore())
            .ForMember(dest => dest.CategoryId, opts => opts.Ignore())
            .ForMember(dest => dest.ExpenseShares, opts => opts.Ignore())
            .ForMember(dest => dest.PaidByMemberId, opts => opts.Ignore())
            .ForMember(dest => dest.PaidByMember, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());

        CreateMap<DbDigests.ExpenseDigest, ExpenseDigest>();

        CreateMap<DbModels.ExpenseShare, ExpenseShare>();
    }
}
