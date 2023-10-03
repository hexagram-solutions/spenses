using AutoMapper;
using Spenses.Application.Models;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Expenses;

public class ExpensesMappingProfile : Profile
{
    public ExpensesMappingProfile()
    {
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
    }
}
