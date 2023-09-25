using BlazorState.Features.JavaScriptInterop;
using BlazorState.Features.Routing;
using BlazorState.Pipeline.ReduxDevTools;
using Microsoft.AspNetCore.Components;

namespace Spenses.Client.Web;

public partial class App : ComponentBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] private JsonRequestHandler JsonRequestHandler { get; set; }
    [Inject] private ReduxDevToolsInterop ReduxDevToolsInterop { get; set; }

    // Injected so it is created by the container. Even though the IDE says it is not used, it is.
    [Inject] private RouteManager RouteManager { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await ReduxDevToolsInterop.InitAsync();
        await JsonRequestHandler.InitAsync();
    }
}
