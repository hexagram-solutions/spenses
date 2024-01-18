using Fluxor;
using MudBlazor;

namespace Spenses.App.Store.Shared;

public class Effects(ISnackbar snackbar)
{
    [EffectMethod]
    public Task HandleApplicationError(ApplicationErrorAction action, IDispatcher _)
    {
        snackbar.Add($"{action.Error.Title}: {action.Error.Details}", Severity.Error);

        return Task.CompletedTask;
    }
}
