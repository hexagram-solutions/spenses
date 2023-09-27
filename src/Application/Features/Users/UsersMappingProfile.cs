using System.Security.Claims;
using AutoMapper;
using Spenses.Application.Models;
using Spenses.Resources.Relational.Models;
using Spenses.Utilities.Security;

namespace Spenses.Application.Features.Users;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<ClaimsPrincipal, UserIdentity>()
            .ForMember(dest => dest.Id,
                opts => opts.MapFrom(src => src.FindFirst(ApplicationClaimTypes.Identifier)!.Value))
            .ForMember(dest => dest.Name,
                opts => opts.MapFrom(src => src.FindFirst(ApplicationClaimTypes.Name)!.Value))
            .ForMember(dest => dest.Issuer,
                opts => opts.MapFrom(src => src.FindFirst(ApplicationClaimTypes.Issuer)!.Value))
            .ForMember(dest => dest.Email,
                opts => opts.MapFrom(src => src.FindFirst(ApplicationClaimTypes.Email)!.Value));

        CreateMap<UserIdentity, User>();
    }
}
