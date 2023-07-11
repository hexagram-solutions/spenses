using AutoMapper;
using Spenses.Domain.Models.Homes;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Homes;

public class HomesMappingProfile : Profile
{
    public HomesMappingProfile()
    {
        CreateMap<HomeProperties, DbModels.Home>()
            .ForMember(dest => dest.Id, opts => opts.Ignore());

        CreateMap<DbModels.Home, Home>();
    }
}
