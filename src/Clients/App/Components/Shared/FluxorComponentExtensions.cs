using Fluxor.Blazor.Web.Components;

namespace Spenses.App.Components.Shared;

public static class FluxorComponentExtensions
{
    public static void SubscribeToAsyncAction<TAction>(this FluxorComponent component, Func<TAction, Task> callback)
    {
        // ReSharper disable once AsyncVoidLambda
        component.SubscribeToAction<TAction>(async a => await callback(a));
    }
}
