using System.Net;
using Microsoft.EntityFrameworkCore;
using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Models.Payments;
using InvitationStatus = Spenses.Resources.Relational.Models.InvitationStatus;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Delete_home_member_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var homeNotFoundResult = await _members.DeleteMember(Guid.NewGuid(), member.Id);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberNotFoundResult = await _members.DeleteMember(home.Id, Guid.NewGuid());

        memberNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_member_with_no_associations_results_in_hard_deletion()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var newMember = (await _members.PostMember(homeId, new CreateMemberProperties
        {
            Name = "Grunky Peep",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = "grunky.peep@georgiasouthern.edu"
        })).Content!;

        var deleteMemberResponse = await _members.DeleteMember(homeId, newMember.Id);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        deleteMemberResponse.Content!.Model.Should().BeEquivalentTo(newMember);

        var memberResponse = await _members.GetMember(homeId, newMember.Id);
        memberResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var members = (await _members.GetMembers(homeId)).Content!;
        members.Should().NotContain(x => x.Id == newMember.Id);

        var homeMembers = (await _homes.GetHome(homeId)).Content!.Members;
        homeMembers.Should().NotContain(x => x.Id == newMember.Id);
    }

    [Fact]
    public async Task Delete_member_with_associations_results_in_deactivation()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var deleteMemberResponse = await _members.DeleteMember(home.Id, member.Id);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteMemberResult = deleteMemberResponse.Content!;

        deleteMemberResult.Model.Should().BeEquivalentTo(member, opts =>
            opts.Excluding(m => m.Status)
                .Excluding(m => m.AvatarUrl));

        deleteMemberResult.Model.Status.Should().Be(MemberStatus.Inactive);

        deleteMemberResult.Type.Should().Be(DeletionType.Deactivated);

        var deactivatedMember = (await _members.GetMember(home.Id, member.Id)).Content!;
        deactivatedMember.Status.Should().Be(MemberStatus.Inactive);

        var members = (await _members.GetMembers(home.Id)).Content!;
        members.Should().Contain(x => x.Id == deactivatedMember.Id && x.Status == MemberStatus.Inactive);

        var homeMembers = (await _homes.GetHome(home.Id)).Content!.Members;
        homeMembers.Should().Contain(x => x.Id == deactivatedMember.Id && x.Status == MemberStatus.Inactive);

        await _members.ActivateMember(home.Id, member.Id);
    }

    [Fact]
    public async Task Delete_member_with_no_associations_and_pending_invitation_deletes_invitation()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var createdMember = (await _members.PostMember(homeId,
            new CreateMemberProperties
            {
                Name = "Grunky Peep",
                DefaultSplitPercentage = 0.0m,
                ContactEmail = "grunky.peep@georgiasouthern.edu"
            })).Content!;

        await _members.PostMemberInvitation(homeId, createdMember.Id,
            new InvitationProperties { Email = createdMember.ContactEmail! });

        var deleteMemberResponse = await _members.DeleteMember(homeId, createdMember.Id);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        deleteMemberResponse.Content!.Type.Should().Be(DeletionType.Deleted);

        await fixture.ExecuteDbContextAction(async db =>
        {
            var invitations = await db.Invitations
                .Where(i => i.MemberId == createdMember.Id)
                .ToListAsync();

            invitations.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task Delete_member_with_associations_and_pending_invitation_cancels_invitation()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var createdMember = (await _members.PostMember(home.Id,
            new CreateMemberProperties
            {
                Name = "Grunky Peep",
                DefaultSplitPercentage = 0.0m,
                ContactEmail = "grunky.peep@georgiasouthern.edu"
            })).Content!;

        await _members.PostMemberInvitation(home.Id, createdMember.Id,
            new InvitationProperties { Email = createdMember.ContactEmail! });

        var payments = RestService.For<IPaymentsApi>(fixture.CreateAuthenticatedClient());

        var paymentResponse = await payments.PostPayment(home.Id, new PaymentProperties
        {
            Amount = 1.00m,
            Date = DateOnly.FromDateTime(DateTime.Today),
            PaidByMemberId = createdMember.Id,
            PaidToMemberId = home.Members.First().Id
        });

        var deleteMemberResponse = await _members.DeleteMember(home.Id, createdMember.Id);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        deleteMemberResponse.Content!.Type.Should().Be(DeletionType.Deactivated);

        await fixture.ExecuteDbContextAction(async db =>
        {
            var invitation = await db.Invitations
                .SingleAsync(i => i.MemberId == createdMember.Id);

            invitation.Status.Should().Be(InvitationStatus.Cancelled);
        });

        await payments.DeletePayment(home.Id, paymentResponse.Content!.Id);
        await _members.DeleteMember(home.Id, createdMember.Id);
    }
}
