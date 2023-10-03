using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Features.Homes;

namespace Spenses.Application.Tests.Features;

public class MappingProfileTests
{
    [Fact]
    public void Mappings_are_valid()
    {
        var services = new ServiceCollection();

        services.AddAutoMapper(typeof(HomesMappingProfile));

        using var serviceProvider = services.BuildServiceProvider();

        var mapper = serviceProvider.GetRequiredService<IMapper>();

        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
