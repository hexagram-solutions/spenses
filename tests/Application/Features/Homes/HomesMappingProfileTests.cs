using AutoMapper;
using Spenses.Application.Features.Homes;
using Spenses.Application.Features.Users;

namespace Spenses.Application.Tests.Features.Homes;

public class HomesMappingProfileTests
{
    [Fact]
    public void Mappings_are_valid()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<HomesMappingProfile>();

            cfg.AddProfile<UsersMappingProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }
}
