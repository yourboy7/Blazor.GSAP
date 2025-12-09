using Microsoft.JSInterop;

namespace Blazor.GSAP.Maui.Components.Pages;
public partial class SplitText {
    /// <summary>
    /// Define the list of plugins required for this component.
    /// Returning an array, for example: [ GsapPlugins.ScrollTrigger, GsapPlugins.Draggable ]
    /// </summary>
    protected override GsapPlugin[] RequiredPlugins { get; } = [GsapPlugin.SplitText];

    /// <summary>
    /// Lifecycle hook invoked after the GSAP core library and the component-specific JavaScript module have been successfully loaded and initialized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Override this method to implement your GSAP animation logic.</strong>
    /// </para>
    /// <para>
    /// This method is intended to be used instead of <see cref="GsapComponentBase.OnAfterRenderAsync"/> for animation initialization.
    /// It guarantees that both the GSAP core and the current component's module (<see cref="GsapComponentBase.JSModule"/>) are available, preventing potential null reference errors during JS interop.
    /// </para>
    /// </remarks>
    protected override async Task OnGsapLoadedAsync() {
        await JSModule.InvokeVoidAsync("init");
    }

    private async Task AnimateChars(){
        await JSModule.InvokeVoidAsync("animateChars");
    }

    private async Task AnimateWords(){
        await JSModule.InvokeVoidAsync("animateWords");
    }

    private async Task AnimateLines(){
        await JSModule.InvokeVoidAsync("animateLines");
    }
}
