using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Net.Http.Json;
using Hexagrams.Extensions.Common.Serialization;
using Refit;
using Spenses.App.Identity.Models;
using Spenses.Application.Models.Authentication;
using Spenses.Client.Http;
using Spenses.Utilities.Security;

namespace Spenses.App.Identity;

/// <summary>
/// Handles state for cookie-based auth.
/// </summary>
public class CookieAuthenticationStateProvider(IAuthApi authApi, IMeApi meApi) : AuthenticationStateProvider, IAccountManagement
{
    /// <summary>
    /// Map the JavaScript-formatted properties to C#-formatted classes.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    /// <summary>
    /// Special auth client.
    /// </summary>
    //private readonly HttpClient _httpClient;

    /// <summary>
    /// Authentication state.
    /// </summary>
    private bool _authenticated;

    /// <summary>
    /// Default principal for anonymous (not authenticated) users.
    /// </summary>
    private readonly ClaimsPrincipal _unauthenticated =
        new(new ClaimsIdentity());

    /// <summary>
    /// Create a new instance of the auth provider.
    /// </summary>
    /// <param name="httpClientFactory">Factory to retrieve auth client.</param>
    //public CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory)
    //    => _httpClient = httpClientFactory.CreateClient("Auth");

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>The result serialized to a <see cref="FormResult"/>.
    /// </returns>
    public async Task<FormResult> RegisterAsync(string email, string password)
    {
        string[] defaultDetail = ["An unknown error prevented registration from succeeding."];

        try
        {

            var result = await authApi.Register(new RegisterRequest { Email = email, Password = password });

            // successful?
            if (result.IsSuccessStatusCode)
            {
                return new FormResult { Succeeded = true };
            }

            // body should contain details about why it failed
            var details = result.Error.Content!.FromJson<ProblemDetails>();
            var errors = new List<string>();
            errors.AddRange(details!.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"));

            // return the error list
            return new FormResult
            {
                Succeeded = false,
                ErrorList = errors.Count == 0 ? defaultDetail : [.. errors]
            };
        }
        catch { }

        // unknown error
        return new FormResult
        {
            Succeeded = false,
            ErrorList = defaultDetail
        };
    }

    public async Task<FormResult> LoginAsync(string email, string password)
    {
        try
        {
            var result = await authApi.Login(new LoginRequest(email, password));

            if (result.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                return new FormResult { Succeeded = true };
            }
        }
        catch { }

        // unknown error
        return new FormResult
        {
            Succeeded = false,
            ErrorList = ["Invalid email and/or password."]
        };
    }

    /// <summary>
    /// Get authentication state.
    /// </summary>
    /// <remarks>
    /// Called by Blazor anytime and authentication-based decision needs to be made, then cached
    /// until the changed state notification is raised.
    /// </remarks>
    /// <returns>The authentication state asynchronous request.</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;

        // default to not authenticated
        var user = _unauthenticated;

        try
        {
            // the user info endpoint is secured, so if the user isn't logged in this will fail
            var userResponse = await meApi.GetMe();

            // throw if user info wasn't retrieved
            if (!userResponse.IsSuccessStatusCode)
            {
                throw new Exception("Login failed"); // todo: find a better thing to do here
            }

            if (userResponse.Content is not null)
            {
                var userInfo = userResponse.Content;

                // in our system name and email are the same
                var claims = new List<Claim>
                {
                    new(ApplicationClaimTypes.Name, userInfo.UserName),
                    new(ApplicationClaimTypes.Email, userInfo.Email),
                    new(ApplicationClaimTypes.EmailVerified, userInfo.EmailVerified.ToString())
                };

                // set the principal
                var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                user = new ClaimsPrincipal(id);
                _authenticated = true;
            }
        }
        catch { }

        // return the state
        return new AuthenticationState(user);
    }

    public async Task LogoutAsync()
    {
        await authApi.Logout();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _authenticated;
    }
}
