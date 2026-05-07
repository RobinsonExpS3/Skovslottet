using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminResident
    {
        [Inject] private HttpClient Http { get; set; } = default!;

        // ── Residents ─────────────────────────────────────────────────────
        private List<EditResidentDTO>?   residents;
        private EditResidentDTO?         selectedResident;
        private bool                     loadFailed  = false;
        private bool                     isBusy      = false;

        // ── Form inputs ───────────────────────────────────────────────────
        private string?          residentNameInput;
        private bool             isActiveInput          = true;
        private Guid?            selectedGroceryDayID;
        private List<Guid>       selectedPaymentMethodIDs = new();
        private List<TimeInput>  medicineTimes            = new();
        private int              selectedHour             = 8;
        private int              selectedMinute           = 0;

        // ── Lookups ───────────────────────────────────────────────────────
        private List<ResidentLookupDTO> groceryDays    = new();
        private List<ResidentLookupDTO> paymentMethods = new();

        // ── Computed ──────────────────────────────────────────────────────
        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(residentNameInput) &&
            selectedGroceryDayID is not null &&
            selectedGroceryDayID != Guid.Empty;

        private bool CanCreate => !isBusy && HasValidInput && selectedResident is null;
        private bool CanUpdate => !isBusy && selectedResident is not null;
        private bool CanDelete => !isBusy && selectedResident is not null;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var response = await httpClient.GetAsync("api/shiftboard/current");

                if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
                {
                    loadErrorMessage = "Du har ikke adgang til denne side.";
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    loadErrorMessage = "Kunne ikke loade vagttavlen. Prøv venligst igen senere.";
                    return;
                }

                //Model = await response.Content.ReadFromJsonAsync<ShiftBoardDTO>();
            }
            catch
            {
                loadErrorMessage = "Kunne ikke loade vagttavlen. Prøv venligst igen senere.";
            }
            finally
            {
                IsLoading = false;
                await LoadDataAsync();
            }
        }

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync() => await LoadDataAsync();

        // ── Data ──────────────────────────────────────────────────────────
        private async Task LoadDataAsync()
        {
            loadFailed = false;
            try
            {
                residents      = await Http.GetFromJsonAsync<List<EditResidentDTO>>("api/Resident/Residents")      ?? new();
                groceryDays    = await Http.GetFromJsonAsync<List<ResidentLookupDTO>>("api/Resident/groceryDays")  ?? new();
                paymentMethods = await Http.GetFromJsonAsync<List<ResidentLookupDTO>>("api/Resident/paymentMethods") ?? new();
            }
            catch
            {
                residents      = new();
                groceryDays    = new();
                paymentMethods = new();
                loadFailed     = true;
            }

            residents = residentResult.Value ?? new();
            groceryDays = groceryDayResult.Value ?? new();
            paymentMethods = paymentMethodResult.Value ?? new();
        }

        private string GetGroceryDayName(Guid id) =>
            groceryDays.FirstOrDefault(d => d.ID == id)?.Name ?? "–";

        private string GetPaymentMethodNames(EditResidentDTO resident)
        {
            if (resident.PaymentMethodIDs is null || resident.PaymentMethodIDs.Count == 0)
                return "–";
            return string.Join(", ", resident.PaymentMethodIDs
                .Select(id => paymentMethods.FirstOrDefault(p => p.ID == id)?.Name ?? "–"));
        }

        // ── Selection ─────────────────────────────────────────────────────
        private async Task SelectResidentAsync(EditResidentDTO resident)
        {
            selectedResident      = resident;
            residentNameInput     = resident.ResidentName;
            selectedGroceryDayID  = resident.GroceryDayID;
            isActiveInput         = resident.IsActive;

            var dto = await Http.GetFromJsonAsync<EditResidentDTO>($"api/Resident/{resident.ResidentID}");
            selectedPaymentMethodIDs = dto?.PaymentMethodIDs ?? new();
            medicineTimes = dto?.MedicineTimes
                .Select(t => new TimeInput { Time = TimeOnly.FromDateTime(t) })
                .ToList() ?? new();
        }

        // ── CRUD ──────────────────────────────────────────────────────────
        private async Task CreateAsync()
        {
            if (!CanCreate) return;
            isBusy = true;
            try
            {
                var dto = new EditResidentDTO
                {
                    ResidentName     = residentNameInput!.Trim(),
                    GroceryDayID     = selectedGroceryDayID!.Value,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes    = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                    IsActive         = isActiveInput
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
                var dto = new EditResidentDTO
                {
                    ResidentID       = selectedResident!.ResidentID,
                    ResidentName     = residentNameInput ?? string.Empty,
                    GroceryDayID     = selectedGroceryDayID ?? Guid.Empty,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes    = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                    IsActive         = isActiveInput
                };
                var response = await Http.PutAsJsonAsync($"api/Resident/{selectedResident.ResidentID}", dto);
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
                var response = await Http.DeleteAsync($"api/Resident/{selectedResident!.ResidentID}");
                if (response.IsSuccessStatusCode) { ClearForm(); await LoadDataAsync(); }
            }
            finally { isBusy = false; }
        }

        private void ClearForm()
        {
            selectedResident         = null;
            residentNameInput        = null;
            selectedGroceryDayID     = null;
            selectedPaymentMethodIDs.Clear();
            medicineTimes.Clear();
            isActiveInput            = true;
        }

        // ── Medicine helpers ──────────────────────────────────────────────
        private void AddMedicineTime()
        {
            var time = new TimeOnly(selectedHour, selectedMinute);
            if (!medicineTimes.Any(t => t.Time == time))
                medicineTimes.Add(new TimeInput { Time = time });
        }

        private void RemoveMedicineTime(TimeInput time) => medicineTimes.Remove(time);

        // ── Payment helper ────────────────────────────────────────────────
        private void OnPaymentMethodsChanged(ChangeEventArgs e)
        {
            selectedPaymentMethodIDs.Clear();
            if (e.Value is string[] arr)
            {
                foreach (var v in arr)
                    if (Guid.TryParse(v, out var id)) selectedPaymentMethodIDs.Add(id);
            }
            else if (e.Value is string s && Guid.TryParse(s, out var single))
            {
                selectedPaymentMethodIDs.Add(single);
            }
        }

        public class TimeInput
        {
            public TimeOnly Time { get; set; } = new(8, 0);
        }
    }
}
