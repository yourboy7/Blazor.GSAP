using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.GSAP;

/// <summary>
/// Inherit from ComponentBase and implement IAsyncDisposable
/// </summary>
public abstract class GsapComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// GSAP Core interop module (refers to the gsap-core.js mentioned above)
    /// </summary>
    private IJSObjectReference? _gsapCoreModule;

    /// <summary>
    /// The business logic module specific to the current page (e.g., Home.razor.js)
    /// </summary>
    private IJSObjectReference? _jsModule;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // 1. Load the core module from the RCL
                _gsapCoreModule = await JS.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/Blazor.GSAP/js/gsap-core.js" // Fixed RCL path format
                );

                // 2. Ensure the GSAP library is loaded and ready
                await _gsapCoreModule.InvokeVoidAsync("initGsap");

                // 3. Automatically load the JS for the current page (Collocated JS)
                // Convention: A [ComponentName].razor.js file must exist in the same directory.
                var jsModulePath = $"./{GetJsModulePath()}";
                _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", jsModulePath);

                // 4. Trigger initialization logic in the subclass
                await OnGsapLoadedAsync(_jsModule);
            }
            catch (JSException ex)    
            {
                Console.WriteLine($"GSAP Init Error: {ex.Message}");
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Lifecycle hook invoked after the GSAP core library and the component-specific JavaScript module have been successfully loaded and initialized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Override this method to implement your GSAP animation logic.</strong>
    /// </para>
    /// <para>
    /// This method is intended to be used instead of <see cref="OnAfterRenderAsync(bool)"/> for animation initialization.
    /// It guarantees that both the GSAP core and the current component's module (<paramref name="jsModule"/>) are available, preventing potential null reference errors during JS interop.
    /// </para>
    /// </remarks>
    /// <param name="jsModule">
    /// A reference to the component-specific JavaScript module (typically corresponding to <c>ComponentName.razor.js</c>).
    /// Use this object to invoke functions exported from your collocated JavaScript file.
    /// </param>
    protected abstract Task OnGsapLoadedAsync(IJSObjectReference jsModule);

    /// <summary>
    /// Automatically calculate the JS path corresponding to the current component
    /// For example, if your page is at Pages/Home.razor, it attempts to load Pages/Home.razor.js
    /// </summary>
    private string GetJsModulePath()
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
        if (_gsapCoreModule is not null)
        {
            // 1. Automatically clean up all GSAP animations
            await _gsapCoreModule.InvokeVoidAsync("killAllGlobal");

            // 2. Dispose the core module
            await _gsapCoreModule.DisposeAsync();
        }

        if (_jsModule is not null)
        {
            // 3. Dispose the page module
            await _jsModule.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}