using AutoMapper;
using Spenses.Application.Features.Users;

namespace Spenses.Application.Tests.Users;

public class UsersMappingProfileTests
{
    [Fact]
    public void Mappings_are_valid()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UsersMappingProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }
}
