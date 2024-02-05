using Refit;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Client.Http;

public interface IMembersApi
{
    [Post("/homes/{homeId}/members")]
    Task<IApiResponse<Member>> PostMember(Guid homeId, CreateMemberProperties props);

    [Get("/homes/{homeId}/members")]
    Task<IApiResponse<IEnumerable<Member>>> GetMembers(Guid homeId);

    [Get("/homes/{homeId}/members/{memberId}")]
    Task<IApiResponse<Member>> GetMember(Guid homeId, Guid memberId);

    [Put("/homes/{homeId}/members/{memberId}")]
    Task<IApiResponse<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props);

    [Delete("/homes/{homeId}/members/{memberId}")]
    Task<IApiResponse<DeletionResult<Member>>> DeleteMember(Guid homeId, Guid memberId);

    [Patch("/homes/{homeId}/members/{memberId}")]
    Task<IApiResponse<Member>> ActivateMember(Guid homeId, Guid memberId);

    [Post("/homes/{homeId}/members/{memberId}/invitations")]
    Task<IApiResponse<Invitation>> PostMemberInvitation(Guid homeId, Guid memberId, InvitationProperties props);

    [Get("/homes/{homeId}/members/{memberId}/invitations")]
    Task<IApiResponse<Invitation[]>> GetMemberInvitations(Guid homeId, Guid memberId);

    [Get("/homes/{homeId}/members/{memberId}/invitations/{invitationId}")]
    Task<IApiResponse<Invitation>> GetMemberInvitation(Guid homeId, Guid memberId, Guid invitationId);

    [Delete("/homes/{homeId}/members/{memberId}/invitations")]
    Task<IApiResponse> CancelMemberInvitations(Guid homeId, Guid memberId);
}
