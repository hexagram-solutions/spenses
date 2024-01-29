using AutoMapper;
using Spenses.Application.Extensions;
using Spenses.Shared.Models.Users;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Users;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<DbModels.ApplicationUser, User>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
            .ForMember(dest => dest.DisplayName, opts => opts.MapFrom(src => src.DisplayName))
            .ForAllOtherMembers(opts => opts.Ignore());
    }
}
