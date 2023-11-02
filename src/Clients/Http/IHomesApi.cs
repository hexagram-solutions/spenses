using Refit;
using Spenses.Application.Models.Homes;

namespace Spenses.Client.Http;

public interface IHomesApi
{
    [Post("/homes")]
    Task<IApiResponse<Home>> PostHome(HomeProperties props);

    [Get("/homes")]
    Task<IApiResponse<IEnumerable<Home>>> GetHomes();

    [Get("/homes/{homeId}")]
    Task<IApiResponse<Home>> GetHome(Guid homeId);

    [Put("/homes/{homeId}")]
    Task<IApiResponse<Home>> PutHome(Guid homeId, HomeProperties props);

    [Delete("/homes/{homeId}")]
    Task<IApiResponse> DeleteHome(Guid homeId);

    [Get("/homes/{homeId}/balance-breakdown")]
    Task<IApiResponse<BalanceBreakdown>> GetBalanceBreakdown(Guid homeId, DateOnly periodStart, DateOnly periodEnd);
}
