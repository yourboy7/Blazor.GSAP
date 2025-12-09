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
    protected IJSObjectReference JSModule { get; private set; } = default!;

    /// <summary>
    /// Define the list of plugins required for this component.
    /// Returning an array, for example: [ GsapPlugins.ScrollTrigger, GsapPlugins.Draggable ]
    /// </summary>
    protected virtual GsapPlugin[] RequiredPlugins => Array.Empty<GsapPlugin>();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                var pluginsToLoad = GetPluginFilenames(RequiredPlugins);

                // 1. Load the core module from the RCL
                _gsapCoreModule = await JS.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/Blazor.GSAP/js/gsap-core.js" // Fixed RCL path format
                );

                // 2. Ensure the GSAP library is loaded and ready
                await _gsapCoreModule.InvokeVoidAsync("initGsap", pluginsToLoad);

                // 3. Automatically load the JS for the current page (Collocated JS)
                // Convention: A [ComponentName].razor.js file must exist in the same directory.
                var jsModulePath = $"./{GetJsModulePath()}";
                JSModule = await JS.InvokeAsync<IJSObjectReference>("import", jsModulePath);

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

    /// <summary>
    /// Lifecycle hook invoked after the GSAP core library and the component-specific JavaScript module have been successfully loaded and initialized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Override this method to implement your GSAP animation logic.</strong>
    /// </para>
    /// <para>
    /// This method is intended to be used instead of <see cref="OnAfterRenderAsync(bool)"/> for animation initialization.
    /// It guarantees that both the GSAP core and the current component's module (<see cref="JSModule"/>) are available, preventing potential null reference errors during JS interop.
    /// </para>
    /// </remarks>
    protected virtual Task OnGsapLoadedAsync() => Task.CompletedTask;

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

    /// <summary>
    /// Convert the enumeration array into a list of filename strings.
    /// </summary>
    private List<string> GetPluginFilenames(GsapPlugin[] plugins)
    {
        // Use HashSet to automatically remove duplicates and prevent the same JS file from being loaded repeatedly.
        var uniqueFilenames = new HashSet<string>();

        foreach (var plugin in plugins)
        {
            switch (plugin)
            {
                // ==========================================
                // 1. Complex dependency handling
                // ==========================================

                // Rule 1: ScrollSmoother requires ScrollTrigger
                case GsapPlugin.ScrollSmoother:
                    uniqueFilenames.Add("ScrollSmoother.min.js");
                    uniqueFilenames.Add("ScrollTrigger.min.js");
                    break;

                // Rule 3: CustomBounce requires CustomEase
                case GsapPlugin.CustomBounce:
                    uniqueFilenames.Add("CustomBounce.min.js");
                    uniqueFilenames.Add("CustomEase.min.js");
                    break;

                // Rule 4: CustomWiggle requires CustomEase
                case GsapPlugin.CustomWiggle:
                    uniqueFilenames.Add("CustomWiggle.min.js");
                    uniqueFilenames.Add("CustomEase.min.js");
                    break;

                // ==========================================
                // 2. Bundles file processing
                // ==========================================

                // Rule 2: RoughEase, ExpoScaleEase, SlowMo -> EasePack.min.js
                case GsapPlugin.RoughEase:
                case GsapPlugin.ExpoScaleEase:
                case GsapPlugin.SlowMo:
                    uniqueFilenames.Add("EasePack.min.js");
                    break;

                // ==========================================
                // 3. Standard mapping
                // ==========================================

                // Files without the "Plugin" extension
                case GsapPlugin.Draggable: uniqueFilenames.Add("Draggable.min.js"); break;
                case GsapPlugin.Flip: uniqueFilenames.Add("Flip.min.js"); break;
                case GsapPlugin.Observer: uniqueFilenames.Add("Observer.min.js"); break;
                case GsapPlugin.ScrollTrigger: uniqueFilenames.Add("ScrollTrigger.min.js"); break;
                case GsapPlugin.SplitText: uniqueFilenames.Add("SplitText.min.js"); break;
                case GsapPlugin.GSDevTools: uniqueFilenames.Add("GSDevTools.min.js"); break;
                case GsapPlugin.CustomEase: uniqueFilenames.Add("CustomEase.min.js"); break;

                // Files with the "Plugin" extension
                case GsapPlugin.DrawSVG: uniqueFilenames.Add("DrawSVGPlugin.min.js"); break;
                case GsapPlugin.Easel: uniqueFilenames.Add("EaselPlugin.min.js"); break;
                case GsapPlugin.Inertia: uniqueFilenames.Add("InertiaPlugin.min.js"); break;
                case GsapPlugin.MotionPath: uniqueFilenames.Add("MotionPathPlugin.min.js"); break;
                case GsapPlugin.MorphSVG: uniqueFilenames.Add("MorphSVGPlugin.min.js"); break;
                case GsapPlugin.Physics2D: uniqueFilenames.Add("Physics2DPlugin.min.js"); break;
                case GsapPlugin.PhysicsProps: uniqueFilenames.Add("PhysicsPropsPlugin.min.js"); break;
                case GsapPlugin.Pixi: uniqueFilenames.Add("PixiPlugin.min.js"); break;
                case GsapPlugin.ScrambleText: uniqueFilenames.Add("ScrambleTextPlugin.min.js"); break;
                case GsapPlugin.ScrollTo: uniqueFilenames.Add("ScrollToPlugin.min.js"); break;
                case GsapPlugin.Text: uniqueFilenames.Add("TextPlugin.min.js"); break;

                // Special naming
                case GsapPlugin.MotionPathHelper: uniqueFilenames.Add("MotionPathHelper.min.js"); break;

                default:
                    // Ignore undefined cases
                    break;
            }
        }

        return uniqueFilenames.ToList();
    }

    public async ValueTask DisposeAsync()
    {
        try {
            if (_gsapCoreModule is not null)
            {
                // 1. Automatically clean up all GSAP animations
                await _gsapCoreModule.InvokeVoidAsync("killAllGlobal");

                // 2. Dispose the core module
                await _gsapCoreModule.DisposeAsync();
            }

            if (JSModule is not null)
            {
                // 3. Dispose the page module
                await JSModule.DisposeAsync();
            }
        } catch (JSException) {
            // Ignore JS exceptions.
            // When the user reloads the page or navigates away, the JavaScript context may already be destroyed.
            // Attempting to invoke JS methods or dispose JS objects at this point can throw "JS object instance ... does not exist".
            // Since the browser has already handled the cleanup, it is safe to ignore this error.
        }

        GC.SuppressFinalize(this);
    }
}