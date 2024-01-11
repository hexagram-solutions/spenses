using System.Security.Claims;
using AutoMapper;
using Spenses.Application.Extensions;
using Spenses.Shared.Models.Users;
using Spenses.Utilities.Security;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Users;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<ClaimsPrincipal, DbModels.ApplicationUser>()
            .ForMember(dest => dest.Id,
                opts => opts.MapFrom(src => src.FindFirst(ApplicationClaimTypes.Identifier)!.Value))
            .ForMember(dest => dest.NickName,
                opts => opts.MapFrom(src => src.FindFirst(ApplicationClaimTypes.NickName)!.Value))
            .ForMember(dest => dest.Email,
                opts => opts.MapFrom(src => src.FindFirst(ApplicationClaimTypes.Email)!.Value))
            .ForAllOtherMembers(opts => opts.Ignore());

        CreateMap<DbModels.ApplicationUser, User>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.UserName))
            .ForAllOtherMembers(opts => opts.Ignore());
    }
}
