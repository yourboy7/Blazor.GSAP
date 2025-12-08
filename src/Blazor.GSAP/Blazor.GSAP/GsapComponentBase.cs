using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.GSAP;

// Inherit from ComponentBase and implement IAsyncDisposable
public abstract class GsapComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    // Core interop module (refers to the gsap-core.js mentioned above)
    private IJSObjectReference? _coreModule;

    // The business logic module specific to the current page (e.g., Home.razor.js)
    protected IJSObjectReference? PageModule { get; private set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // 1. Load the core module from the RCL
                _coreModule = await JS.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/Blazor.GSAP/js/gsap-core.js" // Fixed RCL path format
                );

                // 2. Ensure the GSAP library is loaded and ready
                await _coreModule.InvokeVoidAsync("initGsap");

                // 3. Automatically load the JS for the current page (Collocated JS)
                // Convention: A [ComponentName].razor.js file must exist in the same directory.
                var pageJsPath = $"./{GetJsPath()}";
                PageModule = await JS.InvokeAsync<IJSObjectReference>("import", pageJsPath);

                // 4. Trigger initialization logic in the subclass
                await OnGsapLoadedAsync();
            }
            catch (JSException ex)
            {
                Console.WriteLine($"GSAP Init Error: {ex.Message}");
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    // This is a hook method. Subclasses should override this to execute animations, replacing OnAfterRenderAsync.
    protected virtual Task OnGsapLoadedAsync()
    {
        return Task.CompletedTask;
    }

    // Automatically calculate the JS path corresponding to the current component
    // For example, if your page is at Pages/Home.razor, it attempts to load Pages/Home.razor.js
    private string GetJsPath()
    {
        // Get the specific type of the current component, e.g., "MauiApp1.Components.Pages.Home"
        // Blazor's JS Collocation path is usually relative to wwwroot, like ./Components/Pages/Home.razor.js
        // However, if using 'import "./..."', it is relative to the current URL.
        // The safest approach is to allow subclasses to override this method if the location is non-standard.

        // Simple strategy: Directly return the current component name + .razor.js
        // Note: This requires using relative paths or Blazor's standard isolated JS paths when referencing JS.
        return $"./Components/Pages/{GetType().Name}.razor.js";
    }

    public async ValueTask DisposeAsync()
    {
        if (_coreModule is not null)
        {
            // 1. Automatically clean up all GSAP animations
            await _coreModule.InvokeVoidAsync("killAllGlobal");

            // 2. Dispose the core module
            await _coreModule.DisposeAsync();
        }

        if (PageModule is not null)
        {
            // 3. Dispose the page module
            await PageModule.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}