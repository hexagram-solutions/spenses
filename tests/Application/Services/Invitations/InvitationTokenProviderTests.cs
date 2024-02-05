using Hexagrams.Extensions.Testing;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Services.Invitations;

namespace Spenses.Application.Tests.Services.Invitations;

public class InvitationTokenProviderTests
{
    [Fact]
    public async Task Protected_and_unprotected_data_is_equivalent()
    {
        static Task TestAction(InvitationTokenProvider service)
        {
            var expectedData = new InvitationData(Guid.NewGuid());

            var protectedData = service.ProtectInvitationData(expectedData);

            service.TryUnprotectInvitationData(protectedData, out var actualData);

            actualData.Should().BeEquivalentTo(expectedData);

            return Task.CompletedTask;
        }

        await ServiceTestHarness<InvitationTokenProvider>.Create(TestAction)
            .WithServices(services =>
            {
                services.AddDataProtection();
            })
            .TestAsync();
    }

    [Fact]
    public async Task TryUnprotectInvitationData_returns_false_for_invalid_data()
    {
        static Task TestAction(InvitationTokenProvider service)
        {
            var result = service.TryUnprotectInvitationData("foobar", out var actualData);

            result.Should().BeFalse();
            actualData.Should().BeNull();

            return Task.CompletedTask;
        }

        await ServiceTestHarness<InvitationTokenProvider>.Create(TestAction)
            .WithServices(services =>
            {
                services.AddDataProtection();
            })
            .TestAsync();
    }
}
