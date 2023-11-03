using Refit;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Http;

public interface IMembersApi
{
    [Post("/homes/{homeId}/members")]
    Task<IApiResponse<Member>> PostMember(Guid homeId, MemberProperties props);

    [Get("/homes/{homeId}/members")]
    Task<IApiResponse<IEnumerable<Member>>> GetMembers(Guid homeId);

    [Get("/homes/{homeId}/members/{memberId}")]
    Task<IApiResponse<Member>> GetMember(Guid homeId, Guid memberId);

    [Put("/homes/{homeId}/members/{memberId}")]
    Task<IApiResponse<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props);

    [Delete("/homes/{homeId}/members/{memberId}")]
    Task<IApiResponse> DeleteMember(Guid homeId, Guid memberId);
}
