using System.Security.Claims;

namespace Spenses.Utilities.Security.Services;

public class UserContextProvider
{
    private IUserContext? _userContext;

    public void SetContext(IUserContext context)
    {
        _userContext = context;
    }

    public IUserContext GetContext()
    {
        return _userContext ?? new DummyUserContext();
    }

    private class DummyUserContext : IUserContext
    {
        public ClaimsPrincipal CurrentUser => new();
    }
}

