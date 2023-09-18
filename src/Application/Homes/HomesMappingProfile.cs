using AutoMapper;
using Spenses.Application.Models;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Homes;

public class HomesMappingProfile : Profile
{
    public HomesMappingProfile()
    {
        CreateMap<HomeProperties, DbModels.Home>()
            .ForMember(dest => dest.Id, opts => opts.Ignore());

        CreateMap<DbModels.Home, Home>();

        CreateMap<DbModels.Member, Member>();

        CreateMap<MemberProperties, DbModels.Member>()
            .ForMember(dest => dest.Id, opts => opts.Ignore());
    }
}
