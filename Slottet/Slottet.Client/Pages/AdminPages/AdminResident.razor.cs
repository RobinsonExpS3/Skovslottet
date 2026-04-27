using Microsoft.AspNetCore.Components;
using Slottet.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminResident {
        [Inject]
        public HttpClient? httpClient { get; set; } = default;

        private List<EditResidentDTO>? residents;
        private EditResidentDTO? selectedResident;
        private bool loadFailed = false;
        private Guid? selectedGroceryDayID;
        private List<Guid> selectedPaymentMethodIDs = new();
        private List<ResidentLookupDTO> paymentMethods = new();
        private List<ResidentLookupDTO> groceryDays = new();
        private List<TimeInput> medicineTimes = new();
        private string? residentNameInput;
        private bool _isBusy;

        private int selectedHour = 8;
        private int selectedMinute = 0;

        protected override async Task OnInitializedAsync() {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync() {
            loadFailed = false;

            try {
                residents = await httpClient!.GetFromJsonAsync<List<EditResidentDTO>>("api/Resident/Residents") ?? new();
                groceryDays = await httpClient.GetFromJsonAsync<List<ResidentLookupDTO>>("api/Resident/groceryDays") ?? new();
                paymentMethods = await httpClient.GetFromJsonAsync<List<ResidentLookupDTO>>("api/Resident/paymentMethods") ?? new();

                //var source = residents.FirstOrDefault();
                //groceryDays = source?.GroceryDays?.ToList() ?? new();
                //paymentMethods = source?.PaymentMethods?.ToList() ?? new();
            } catch {
                residents = new();
                groceryDays = new();
                paymentMethods = new();
                loadFailed = true;
            }
        }

        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(residentNameInput)
            && selectedGroceryDayID != null
            && selectedResident == null;

        private bool CanCreate => !_isBusy && HasValidInput;
        private bool CanUpdate => !_isBusy && selectedResident != null;
        private bool CanDelete => !_isBusy && selectedResident != null;

        private async Task SelectResidentAsync(EditResidentDTO resident) {
            selectedResident = resident;
            residentNameInput = resident.ResidentName;
            selectedGroceryDayID = resident.GroceryDayID;

            var dto = await httpClient!.GetFromJsonAsync<EditResidentDTO>($"api/Resident/{resident.ResidentID}");
            selectedPaymentMethodIDs = dto?.PaymentMethodIDs ?? new();
            medicineTimes = dto?.MedicineTimes
                .Select(t => new TimeInput { Time = TimeOnly.FromDateTime(t) })
                .ToList() ?? new();

            //groceryDays = dto?.GroceryDays?.ToList() ?? groceryDays;
            //paymentMethods = dto?.PaymentMethods?.ToList() ?? paymentMethods;
        }

        private async Task CreateAsync() {
            if(!CanCreate) return;
            _isBusy = true;

            try {
                var dto = new EditResidentDTO {
                    ResidentName = residentNameInput ?? string.Empty,
                    GroceryDayID = selectedGroceryDayID ?? Guid.Empty,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                    IsActive = true
                };

                var response = await httpClient.PostAsJsonAsync("api/Resident", dto);

                if (!response.IsSuccessStatusCode) {
                    return;
                }

                ClearForm();
                await LoadDataAsync();
            } finally {
                _isBusy = false;
            }
        }


        private async Task UpdateAsync() {
            if (selectedResident == null) return;

            if(!CanUpdate) return;
            _isBusy = true;

            try {
                var dto = new EditResidentDTO {
                    ResidentID = selectedResident.ResidentID,
                    ResidentName = residentNameInput ?? string.Empty,
                    GroceryDayID = selectedGroceryDayID ?? Guid.Empty,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                    IsActive = selectedResident.IsActive
                };

                var response = await httpClient.PutAsJsonAsync($"api/Resident/{selectedResident.ResidentID}", dto);

                if (!response.IsSuccessStatusCode) {
                    return;
                }

                ClearForm();
                await LoadDataAsync();
            } finally {
                _isBusy = false; 
            }
        }

        private async Task DeleteAsync() {
            if (selectedResident == null) return;

            if(!CanDelete) return;
            _isBusy = true;

            try {
                var response = await httpClient.DeleteAsync($"api/Resident/{selectedResident.ResidentID}");

                if (!response.IsSuccessStatusCode) {
                    return;
                }

                ClearForm();
                await LoadDataAsync();
            } finally {
                _isBusy = false;
            }
        }

        private void OnPaymentMethodsChanged(ChangeEventArgs e) {
            selectedPaymentMethodIDs.Clear();

            if (e.Value is string[] valuesArray) {
                foreach (var v in valuesArray)
                    if (Guid.TryParse(v, out var id))
                        selectedPaymentMethodIDs.Add(id);
            } else if (e.Value is string singleValue && Guid.TryParse(singleValue, out var id)) {
                selectedPaymentMethodIDs.Add(id);
            }
        }

        public class TimeInput {
            public TimeOnly Time { get; set; } = new(8, 0);
        }

        private void RemoveMedicineTime(TimeInput time) {
            medicineTimes.Remove(time);
        }

        private void AddMedicineTime() {
            var time = new TimeOnly(selectedHour, selectedMinute);

            if (!medicineTimes.Any(t => t.Time == time)) {
                medicineTimes.Add(new TimeInput { Time = time });
            }
        }

        private string GetGroceryDayName(Guid groceryDayId) {
            return groceryDays.FirstOrDefault(day => day.ID == groceryDayId)?.Name ?? "Ukendt dag";
        }

        private void ClearForm() {
            selectedResident = null;
            residentNameInput = null;
            selectedGroceryDayID = null;
            selectedPaymentMethodIDs.Clear();
            medicineTimes.Clear();
        }
    }
}