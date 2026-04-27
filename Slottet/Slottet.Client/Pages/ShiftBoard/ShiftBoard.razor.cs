using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Slottet.Shared;

namespace Slottet.Client.Pages.ShiftBoard
{
    public partial class ShiftBoard
    {
        // ── DI ────────────────────────────────────────────────────────────
        [Inject] private IJSRuntime  JS   { get; set; } = default!;
        [Inject] private HttpClient  Http { get; set; } = default!;

        // ── Data ──────────────────────────────────────────────────────────
        protected ShiftBoardDTO? Model     { get; set; }
        protected bool           IsLoading { get; set; } = true;
        protected string?        LoadError { get; set; }

        // ── Overlay state ─────────────────────────────────────────────────
        protected ResidentCardDto? SelectedResident { get; set; }
        protected OverlayPanel     ActivePanel      { get; set; } = OverlayPanel.None;

        // ── FLIP / animation state ────────────────────────────────────────
        private BoundingRect? _cardRect;
        private bool          _pendingFlyIn;
        private bool          _pendingSidebarFlyIn;
        private string        _sidebarPanelId = string.Empty;

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await Http.GetFromJsonAsync<ShiftBoardDTO>("api/shiftboard/current");
            }
            catch (Exception ex)
            {
                LoadError = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// After Blazor has rendered the panel (still opacity:0) we call JS
        /// which measures the panel and starts the FLIP animation.
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_pendingFlyIn && _cardRect is not null)
            {
                _pendingFlyIn = false;
                await JS.InvokeVoidAsync("overlayHelpers.playFlyIn", "overlay-panel-borger", _cardRect);
            }

            if (_pendingSidebarFlyIn && !string.IsNullOrEmpty(_sidebarPanelId))
            {
                _pendingSidebarFlyIn = false;
                await JS.InvokeVoidAsync("overlayHelpers.playFlyIn", _sidebarPanelId, null);
            }
        }

        // ── Overlay handlers ──────────────────────────────────────────────

        /// <summary>
        /// Measures the card's position in the DOM BEFORE showing the overlay panel,
        /// so the FLIP animation can start from there.
        /// </summary>
        protected async Task OpenResident(ResidentCardDto resident, string cardId)
        {
            _cardRect        = await JS.InvokeAsync<BoundingRect>("overlayHelpers.getRect", cardId);
            SelectedResident = resident;
            _pendingFlyIn    = true;
            ActivePanel      = OverlayPanel.None;
        }

        protected void OpenSidebarPanel(OverlayPanel panel)
        {
            SelectedResident     = null;
            ActivePanel          = panel;
            _sidebarPanelId      = "overlay-panel-" + panel.ToString().ToLower();
            _pendingSidebarFlyIn = true;
        }

        protected void CloseOverlay()
        {
            SelectedResident = null;
            ActivePanel      = OverlayPanel.None;
        }

        // ── Resident saved callback ───────────────────────────────────────
        /// <summary>
        /// Called by ResidentCard when the user clicks "Gem ændringer".
        /// Closes the overlay — Blazor re-renders ShiftBoard, picking up the
        /// mutated Resident object and refreshing the grid card.
        /// </summary>
        protected void OnResidentSaved() => CloseOverlay();

        // ── Navigation ────────────────────────────────────────────────────
        private void NavigateToNewShiftBoard()
        {
            // TODO: NavigationManager.NavigateTo("/shiftboard/new");
        }
    }

    /// <summary>Enum of sidebar panels that can be opened in the overlay.</summary>
    public enum OverlayPanel { None, PhoneList, DepartmentTasks, SpecialResponsibilities }

    /// <summary>JS interop DTO for getBoundingClientRect.</summary>
    public record BoundingRect(double X, double Y, double Width, double Height);
}
