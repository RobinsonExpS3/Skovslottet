using Microsoft.AspNetCore.Components;

namespace Slottet.Client.Components.Overlay
{
    public partial class Overlay
    {
        // ── Parameters ────────────────────────────────────────────────────
        [Parameter, EditorRequired] public bool            IsVisible    { get; set; }
        [Parameter, EditorRequired] public EventCallback   OnClose      { get; set; }
        [Parameter]                 public RenderFragment? ChildContent  { get; set; }

        /// <summary>Unikt DOM-id på panelet — bruges af JS FLIP-animation.</summary>
        [Parameter] public string PanelId { get; set; } = "overlay-panel";

        /// <summary>"sm" | "md" (default) | "lg" | "full"</summary>
        [Parameter] public string Size { get; set; } = "md";

        // ── Computed ──────────────────────────────────────────────────────
        private string SizeClass => Size switch
        {
            "sm"   => "overlay-panel--sm",
            "lg"   => "overlay-panel--lg",
            "full" => "overlay-panel--full",
            _      => "overlay-panel--md"
        };

        // ── Close ─────────────────────────────────────────────────────────
        private async Task HandleClose() => await OnClose.InvokeAsync();
    }
}
