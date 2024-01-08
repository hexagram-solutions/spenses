using Refit;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Client.Http;

public interface IMeApi
{
    [Get("/me/info")]
    public Task<IApiResponse<CurrentUser>> GetMe();

    [Put("/me/info")]
    public Task<IApiResponse<CurrentUser>> UpdateMe();
}
