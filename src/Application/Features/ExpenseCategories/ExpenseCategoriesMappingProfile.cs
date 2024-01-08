using AutoMapper;
using Spenses.Shared.Models.ExpenseCategories;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.ExpenseCategories;

public class ExpenseCategoriesMappingProfile : Profile
{
    public ExpenseCategoriesMappingProfile()
    {
        CreateMap<DbModels.ExpenseCategory, ExpenseCategory>();

        CreateMap<ExpenseCategoryProperties, DbModels.ExpenseCategory>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.IsDefault, opts => opts.Ignore())
            .ForMember(dest => dest.Home, opts => opts.Ignore())
            .ForMember(dest => dest.HomeId, opts => opts.Ignore())
            .ForMember(dest => dest.Expenses, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedById, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedBy, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedById, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedBy, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.ModifiedAt, opts => opts.Ignore());
    }
}
