using Refit;
using Spenses.Application.Models;

namespace Spenses.Client.Http;

public interface IHomesApi
{
    [Post("/homes")]
    Task<Home> PostHome(HomeProperties props);

    [Get("/homes")]
    Task<IEnumerable<Home>> GetHomes();

    [Get("/homes/{homeId}")]
    Task<Home> GetHome(Guid homeId);

    [Put("/homes/{homeId}")]
    Task<Home> PutHome(Guid homeId, HomeProperties props);

    [Delete("/homes/{homeId}")]
    Task DeleteHome(Guid homeId);
}
