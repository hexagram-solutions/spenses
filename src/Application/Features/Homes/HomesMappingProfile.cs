using AutoMapper;
using Spenses.Application.Models;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Homes;

public class HomesMappingProfile : Profile
{
    public HomesMappingProfile()
    {
        CreateMap<DbModels.Home, Home>();

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
    }
}
