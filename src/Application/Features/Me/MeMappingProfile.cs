using AutoMapper;
using Spenses.Application.Extensions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;

namespace Spenses.Application.Features.Me;

public class MeMappingProfile : Profile
{
    public MeMappingProfile()
    {
        CreateMap<ApplicationUser, CurrentUser>()
            .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.Email))
            .ForMember(dest => dest.NickName, opts => opts.MapFrom(src => src.NickName))
            .ForMember(dest => dest.EmailVerified, opts => opts.MapFrom(src => src.EmailConfirmed));

        CreateMap<UserProfileProperties, ApplicationUser>()
            .ForMember(dest => dest.NickName, opts => opts.MapFrom(src => src.NickName))
            .ForAllOtherMembers(opts => opts.Ignore());
    }
}
