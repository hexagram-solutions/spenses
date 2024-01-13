using Refit;
using Spenses.Shared.Models.Me;

namespace Spenses.Client.Http;

public interface IMeApi
{
    [Get("/me")]
    public Task<IApiResponse<CurrentUser>> GetMe();

    [Put("/me")]
    public Task<IApiResponse<CurrentUser>> UpdateMe(UserProfileProperties props);
}
