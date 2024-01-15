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
            .ForMember(dest => dest.DisplayName, opts => opts.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.EmailVerified, opts => opts.MapFrom(src => src.EmailConfirmed));

        CreateMap<UserProfileProperties, ApplicationUser>()
            .ForMember(dest => dest.DisplayName, opts => opts.MapFrom(src => src.DisplayName))
            .ForAllOtherMembers(opts => opts.Ignore());
    }
}
