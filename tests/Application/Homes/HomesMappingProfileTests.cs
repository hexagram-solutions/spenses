using AutoMapper;
using Spenses.Application.Features.Homes;

namespace Spenses.Application.Tests.Homes;

public class HomesMappingProfileTests
{
    [Fact]
    public void Mappings_are_valid()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<HomesMappingProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }
}
