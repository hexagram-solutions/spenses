using Refit;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Http;

public interface IMembersApi
{
    [Post("/homes/{homeId}/members")]
    Task<ApiResponse<Member>> PostMember(Guid homeId, MemberProperties props);

    [Get("/homes/{homeId}/members")]
    Task<ApiResponse<IEnumerable<Member>>> GetMembers(Guid homeId);

    [Get("/homes/{homeId}/members/{memberId}")]
    Task<ApiResponse<Member>> GetMember(Guid homeId, Guid memberId);

    [Put("/homes/{homeId}/members/{memberId}")]
    Task<ApiResponse<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props);

    [Delete("/homes/{homeId}/members/{memberId}")]
    Task DeleteMember(Guid homeId, Guid memberId);
}
