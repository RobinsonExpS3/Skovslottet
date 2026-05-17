using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Slottet.Client.Pages.AdminPages;
using Slottet.Shared;

namespace Slottet.Client.Pages.ShiftBoard
{
    public partial class ShiftBoard
    {
        // ── DI ────────────────────────────────────────────────────────────
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        // ── Data ──────────────────────────────────────────────────────────
        protected ShiftBoardDTO? Model { get; set; }
        protected bool IsLoading = false;
        protected string? LoadError { get; set; }

        // ── Navigation ────────────────────────────────────────────────────
        private DateOnly _navDate;
        private string _navShift = string.Empty;

        protected bool IsPast =>
            Model is not null && Model.EndDate < DateTime.Now;

        protected string ShiftLabel => _navShift switch
        {
            "Dag" => "🌤 Dag (07–15)",
            "Aften" => "🌆 Aften (15–23)",
            "Nat" => "🌙 Nat (23–07)",
            _ => _navShift
        };

        protected string NavDayLabel => _navDate.DayOfWeek switch
        {
            DayOfWeek.Monday => "Mandag",
            DayOfWeek.Tuesday => "Tirsdag",
            DayOfWeek.Wednesday => "Onsdag",
            DayOfWeek.Thursday => "Torsdag",
            DayOfWeek.Friday => "Fredag",
            DayOfWeek.Saturday => "Lørdag",
            DayOfWeek.Sunday => "Søndag",
            _ => string.Empty
        };

        protected bool IsToday =>
            _navDate == DateOnly.FromDateTime(DateTime.Today) &&
            _navShift == CurrentShiftType();

        // ── Overlay state ─────────────────────────────────────────────────
        protected ResidentCardDto? SelectedResident { get; set; }
        protected OverlayPanel ActivePanel { get; set; } = OverlayPanel.None;

        // ── FLIP / animation state ────────────────────────────────────────
        private BoundingRect? _cardRect;
        private bool _pendingFlyIn;
        private bool _pendingSidebarFlyIn;
        private string _sidebarPanelId = string.Empty;

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            (_navDate, _navShift) = GetCurrentNavState();

            try
            {
                using var response = await Http.GetAsync("api/shiftboard/current");

                if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
                {
                    LoadError = "Du har ikke adgang til denne side.";
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    LoadError = "Kunne ikke loade vagttavlen. Prøv venligst igen senere.";
                    return;
                }

                //Model = await response.Content.ReadFromJsonAsync<ShiftBoardDTO>();
            }
            catch
            {
                LoadError = "Kunne ikke loade vagttavlen. Prøv venligst igen senere. - ShiftBoard";
            }
            finally
            {
                IsLoading = false;
                await LoadShiftAsync();
            }
        }

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

        // ── Navigation ────────────────────────────────────────────────────
        protected async Task GoToPrev()
        {
            if (Model is null) return;
            await LoadShiftByUrlAsync($"api/shiftboard/{Model.ShiftBoardId}/previous");
        }

        protected async Task GoToNext()
        {
            if (Model is null) return;
            await LoadShiftByUrlAsync($"api/shiftboard/{Model.ShiftBoardId}/next-or-create");
        }

        protected async Task GoToToday()
        {
            (_navDate, _navShift) = GetCurrentNavState();
            await LoadShiftAsync();
        }

        // TODO: Skift til dedikeret oprettelses-side når den findes
        protected void NavigateToNewShiftBoard() =>
            Nav.NavigateTo("/adminStaff");

        private async Task LoadShiftAsync()
        {
            var url = $"api/shiftboard/by-shift?date={_navDate:yyyy-MM-dd}&shiftType={_navShift}";
            await LoadShiftByUrlAsync(url);
        }

        private async Task LoadShiftByUrlAsync(string url)
        {
            IsLoading = true;
            LoadError = null;
            Model = null;
            CloseOverlay();

            try
            {
                var result = await AdminHttp.GetJsonAsync<ShiftBoardDTO>(Http, url);

                if (result.Failed)
                {
                    LoadError = result.ErrorMessage;
                    return;
                }
                Model = result.Value;
                if (Model is not null)
                {
                    _navDate = DateOnly.FromDateTime(Model.StartDate);
                    _navShift = Model.ShiftType;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ── Shift helpers ─────────────────────────────────────────────────
        private static (DateOnly date, string shift) GetCurrentNavState()
        {
            var now = DateTime.Now;
            var shift = CurrentShiftType(now);
            var date = (now.Hour < 7)
                ? DateOnly.FromDateTime(now.AddDays(-1))   // Nat started yesterday
                : DateOnly.FromDateTime(now);
            return (date, shift);
        }

        private static string CurrentShiftType(DateTime? at = null)
        {
            var hour = (at ?? DateTime.Now).Hour;
            return hour switch
            {
                >= 7 and < 15 => "Dag",
                >= 15 and < 23 => "Aften",
                _ => "Nat"
            };
        }

        // ── Overlay handlers ──────────────────────────────────────────────
        protected async Task OpenResident(ResidentCardDto resident, string cardId)
        {
            if (IsPast) return;
            _cardRect = await JS.InvokeAsync<BoundingRect>("overlayHelpers.getRect", cardId);
            SelectedResident = resident;
            _pendingFlyIn = true;
            ActivePanel = OverlayPanel.None;
        }

        protected void OpenSidebarPanel(OverlayPanel panel)
        {
            SelectedResident = null;
            ActivePanel = panel;
            _sidebarPanelId = "overlay-panel-" + panel.ToString().ToLower();
            _pendingSidebarFlyIn = true;
        }

        protected void CloseOverlay()
        {
            SelectedResident = null;
            ActivePanel = OverlayPanel.None;
        }

        protected async Task OnResidentSaved()
        {
            if (SelectedResident is not null)
            {
                try
                {
                    await Http.PatchAsJsonAsync("api/shiftboard/resident-card", SelectedResident);
                }
                catch
                {
                    // TODO: surface save error to user
                }
            }

            CloseOverlay();
            await LoadShiftAsync();
        }
    }

    public enum OverlayPanel { None, PhoneList, DepartmentTasks, SpecialResponsibilities }
    public record BoundingRect(double X, double Y, double Width, double Height);
}
