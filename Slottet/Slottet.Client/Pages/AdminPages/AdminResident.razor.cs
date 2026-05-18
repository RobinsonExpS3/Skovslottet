using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminResident
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // ── Data ──────────────────────────────────────────────────────────
        protected List<ResidentCardDto> Cards { get; set; } = new();
        protected List<string> AllStaff { get; set; } = new();
        protected bool IsLoading { get; set; } = true;
        protected string? LoadError { get; set; }

        // ── Overlay ───────────────────────────────────────────────────────
        protected ResidentCardDto? SelectedResident { get; set; }
        private AdminBoundingRect? _cardRect;
        private bool _pendingFlyIn;
        private bool _isNewResident;

        // ── Drag & drop ───────────────────────────────────────────────────
        private int _dragSourceSlot = -1;
        private int _dragTargetSlot = -1;

        // ── Creation form ─────────────────────────────────────────────────
        private string? residentNameInput;
        private bool isActiveInput = true;
        private Guid? selectedGroceryDayID;
        private List<Guid> selectedPaymentMethodIDs = new();
        private List<TimeInput> medicineTimes = new();
        private int selectedHour = 8;
        private int selectedMinute = 0;
        private bool isBusy = false;

        // ── Lookups ───────────────────────────────────────────────────────
        private List<ResidentLookupDTO> groceryDays = new();
        private List<ResidentLookupDTO> paymentMethods = new();



        // ── Computed ──────────────────────────────────────────────────────
        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(residentNameInput) &&
            selectedGroceryDayID is not null &&
            selectedGroceryDayID != Guid.Empty;

        private bool CanCreate => !isBusy && HasValidInput;
        private bool CanUpdate => !isBusy && SelectedResident is not null;
        private bool CanDelete => !isBusy && SelectedResident is not null;
        protected override async Task OnInitializedAsync()
        {
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
                    LoadError = "Kunne ikke loade siden. Prøv venligst igen senere.";
                    return;
                }

                //Model = await response.Content.ReadFromJsonAsync<ShiftBoardDTO>();
            }
            finally
            {
                isBusy = false;

                if (LoadError is null)
                {
                    await LoadDataAsync();
                }
                else
                {
                    IsLoading = false;
                }
            }
        }

        // ── Lifecycle ─────────────────────────────────────────────────────

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_pendingFlyIn && _cardRect is not null)
            {
                _pendingFlyIn = false;
                await JS.InvokeVoidAsync("overlayHelpers.playFlyIn", "overlay-panel-admin-borger", _cardRect);
            }
        }

        // ── Data loading ──────────────────────────────────────────────────
        private async Task LoadDataAsync()
        {
            IsLoading = true;
            LoadError = null;
            try
            {
                var cardsTask = Http.GetFromJsonAsync<List<ResidentCardDto>>("api/Resident/cards");
                var staffTask = Http.GetFromJsonAsync<List<EditStaffDto>>("api/Staff/Staffs");
                var groceryTask = Http.GetFromJsonAsync<List<ResidentLookupDTO>>("api/Resident/groceryDays");
                var paymentTask = Http.GetFromJsonAsync<List<ResidentLookupDTO>>("api/Resident/paymentMethods");

                await Task.WhenAll(cardsTask, staffTask, groceryTask, paymentTask);

                Cards = cardsTask.Result ?? new();
                AllStaff = (staffTask.Result ?? new()).Select(s => s.StaffName).ToList();
                groceryDays = groceryTask.Result ?? new();
                paymentMethods = paymentTask.Result ?? new();
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

        // ── Overlay ───────────────────────────────────────────────────────
        protected async Task OpenResident(ResidentCardDto resident, string cardId)
        {
            _cardRect        = await JS.InvokeAsync<AdminBoundingRect>("overlayHelpers.getRect", cardId);
            SelectedResident = resident;
            _isNewResident   = false;
            _pendingFlyIn    = true;
        }

        protected async Task OpenNewResident(string cardId)
        {
            _cardRect = await JS.InvokeAsync<AdminBoundingRect>("overlayHelpers.getRect", cardId);
            SelectedResident = new ResidentCardDto
            {
                ResidentID       = Guid.Empty,
                ResidentName     = string.Empty,
                GroceryDay       = string.Empty,
                IsActive         = true,
                MedicineSchedule = new(),
                PNEntries        = new(),
                AssignedStaff    = new(),
            };
            _isNewResident = true;
            _pendingFlyIn  = true;
        }

        protected void CloseOverlay()
        {
            SelectedResident = null;
            _isNewResident   = false;
        }

        protected async Task OnResidentDeleted()
        {
            if (SelectedResident is not null)
            {
                isBusy = true;
                try
                {
                    await Http.PatchAsync(
                        $"api/Resident/{SelectedResident.ResidentID}/deactivate",
                        content: null);
                }
                finally { isBusy = false; }
            }
            CloseOverlay();
            await LoadDataAsync();
        }

        protected async Task OnResidentSaved()
        {
            if (_isNewResident)
            {
                await CreateFromCardAsync();
                return;
            }

            if (SelectedResident is not null)
            {
                var times = SelectedResident.MedicineSchedule
                    .Select(m => m.Time)
                    .ToList();

                await Http.PatchAsJsonAsync(
                    $"api/Resident/{SelectedResident.ResidentID}/medicine-times",
                    times);

                var groceryDayId = groceryDays
                    .FirstOrDefault(g => g.Name == SelectedResident.GroceryDay)?.ID;

                if (groceryDayId.HasValue && groceryDayId.Value != Guid.Empty)
                {
                    await Http.PatchAsJsonAsync(
                        $"api/Resident/{SelectedResident.ResidentID}/grocery-day",
                        groceryDayId.Value);
                }

                await Http.PatchAsJsonAsync(
                    $"api/Resident/{SelectedResident.ResidentID}/payment-methods",
                    SelectedResident.PaymentMethodIDs);
            }

            CloseOverlay();
            await LoadDataAsync();
        }

        private async Task CreateFromCardAsync()
        {
            if (SelectedResident is null) return;

            var groceryDayId = groceryDays
                .FirstOrDefault(g => g.Name == SelectedResident.GroceryDay)?.ID
                ?? Guid.Empty;

            var dto = new EditResidentDto
            {
                ResidentName     = SelectedResident.ResidentName.Trim(),
                GroceryDayID     = groceryDayId,
                PaymentMethodIDs = SelectedResident.PaymentMethodIDs.ToList(),
                MedicineTimes    = SelectedResident.MedicineSchedule.Select(m => m.Time).ToList(),
                IsActive         = true,
            };

            isBusy = true;
            try
            {
                var response = await Http.PostAsJsonAsync("api/Resident", dto);
                if (response.IsSuccessStatusCode)
                {
                    CloseOverlay();
                    await LoadDataAsync();
                }
            }
            finally { isBusy = false; }
        }

        // ── Creation CRUD ─────────────────────────────────────────────────
        private async Task CreateAsync()
        {
            if (!CanCreate) return;
            isBusy = true;
            try
            {
                var dto = new EditResidentDto
                {
                    ResidentName = residentNameInput!.Trim(),
                    GroceryDayID = selectedGroceryDayID!.Value,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes = medicineTimes.Select(t => t.Time).ToList(),
                    IsActive = isActiveInput
                };
                var response = await Http.PostAsJsonAsync("api/Resident", dto);
                if (response.IsSuccessStatusCode) { ClearForm(); await LoadDataAsync(); }
            }
            finally { isBusy = false; }
        }

        private async Task UpdateAsync()
        {
            if (!CanUpdate) return;
            isBusy = true;
            try
            {
                var dto = new EditResidentDto
                {
                    ResidentID = SelectedResident!.ResidentID,
                    ResidentName = residentNameInput ?? string.Empty,
                    GroceryDayID = selectedGroceryDayID ?? Guid.Empty,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes = medicineTimes.Select(t => t.Time).ToList(),
                    IsActive = isActiveInput
                };
                var response = await Http.PutAsJsonAsync($"api/Resident/{SelectedResident.ResidentID}", dto);
                if (response.IsSuccessStatusCode) { ClearForm(); await LoadDataAsync(); }
            }
            finally { isBusy = false; }
        }

        private async Task DeleteAsync()
        {
            if (!CanDelete) return;
            isBusy = true;
            try
            {
                var response = await Http.DeleteAsync($"api/Resident/{SelectedResident!.ResidentID}");
                if (response.IsSuccessStatusCode) { ClearForm(); await LoadDataAsync(); }
            }
            finally { isBusy = false; }
        }



        // ── Drag & drop ───────────────────────────────────────────────────
        private void OnDragStart(int slot)
        {
            _dragSourceSlot = slot;
            _dragTargetSlot = -1;
        }

        private void OnDragEnd()
        {
            _dragSourceSlot = -1;
            _dragTargetSlot = -1;
        }

        private async Task OnDrop(int targetSlot)
        {
            var src = _dragSourceSlot;
            _dragSourceSlot = -1;
            _dragTargetSlot = -1;

            if (src < 0 || src == targetSlot) return;
            if (src >= Cards.Count || targetSlot >= Cards.Count) return;

            // Optimistisk swap i UI
            var tmp = Cards[src];
            Cards[src] = Cards[targetSlot];
            Cards[targetSlot] = tmp;

            // Persist til API
            var dto = new Slottet.Shared.SwapResidentSortOrderDto
            {
                ResidentIdA = Cards[targetSlot].ResidentID, // nu på src-plads
                ResidentIdB = Cards[src].ResidentID          // nu på target-plads
            };

            var response = await Http.PutAsJsonAsync("api/Resident/swap-order", dto);
            if (!response.IsSuccessStatusCode)
            {
                // Rul tilbage ved fejl
                var revert = Cards[src];
                Cards[src] = Cards[targetSlot];
                Cards[targetSlot] = revert;
            }
        }

        private void ClearForm()
        {
            residentNameInput = null;
            selectedGroceryDayID = null;
            selectedPaymentMethodIDs.Clear();
            medicineTimes.Clear();
            isActiveInput = true;
        }

        // ── Medicine helpers ──────────────────────────────────────────────
        private void AddMedicineTime()
        {
            var time = new TimeOnly(selectedHour, selectedMinute);
            if (!medicineTimes.Any(t => t.Time == time))
                medicineTimes.Add(new TimeInput { Time = time });
        }

        private void RemoveMedicineTime(TimeInput time) => medicineTimes.Remove(time);

        private void OnPaymentMethodsChanged(ChangeEventArgs e)
        {
            selectedPaymentMethodIDs.Clear();
            if (e.Value is string[] arr)
                foreach (var v in arr)
                    if (Guid.TryParse(v, out var id)) selectedPaymentMethodIDs.Add(id);
                    else if (e.Value is string s && Guid.TryParse(s, out var single))
                        selectedPaymentMethodIDs.Add(single);
        }

        public class TimeInput
        {
            public TimeOnly Time { get; set; } = new(8, 0);
        }
    }

    public record AdminBoundingRect(double X, double Y, double Width, double Height);
}
