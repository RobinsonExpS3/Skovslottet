using Microsoft.AspNetCore.Components;
using Slottet.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class EditResident {
        [Inject]
        public HttpClient? httpClient { get; set; } = default;

        private List<ResidentViewModel>? residents;
        private ResidentViewModel? selectedResident;
        private bool loadFailed = false;
        private Guid? selectedGroceryDayID;
        private List<Guid> selectedPaymentMethodIDs = new();
        private List<PaymentMethodDTO> paymentMethods = new();
        private List<GroceryDayDTO> groceryDays = new();
        private List<TimeInput> medicineTimes = new();
        private string? residentNameInput;

        private int selectedHour = 8;
        private int selectedMinute = 0;

        protected override async Task OnInitializedAsync() {
            await LoadData();
        }

        private async Task LoadData() {
            loadFailed = false;

            try {
                residents = await httpClient!.GetFromJsonAsync<List<ResidentViewModel>>("api/Resident/Residents") ?? new();
            } catch {
                residents = new();
                loadFailed = true;
            }

            try {
                groceryDays = await httpClient!.GetFromJsonAsync<List<GroceryDayDTO>>("api/GroceryDay") ?? new();
            } catch {
                groceryDays = new();
                loadFailed = true;
            }

            try {
                paymentMethods = await httpClient!.GetFromJsonAsync<List<PaymentMethodDTO>>("api/PaymentMethod") ?? new();
            } catch {
                paymentMethods = new();
                loadFailed = true;
            }
        }

        private async Task SelectResident(ResidentViewModel resident) {
            selectedResident = resident;
            residentNameInput = resident.ResidentName;
            selectedGroceryDayID = resident.GroceryDayID;

            var dto = await httpClient.GetFromJsonAsync<ResidentDTO>($"api/Resident/{resident.ResidentID}");
            selectedPaymentMethodIDs = dto?.PaymentMethodIDs ?? new();
            medicineTimes = dto?.MedicineTimes
                .Select(t => new TimeInput { Time = TimeOnly.FromDateTime(t) })
                .ToList() ?? new();
        }

        private async Task Create() {
            var dto = new ResidentDTO {
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
            await LoadData();
        }


        private async Task Update() {
            if (selectedResident == null) {
                return;
            }

            var dto = new ResidentDTO {
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
            await LoadData();
        }

        private async Task Delete() {
            if (selectedResident == null) {
                return;
            }

            var response = await httpClient.DeleteAsync($"api/Resident/{selectedResident.ResidentID}");

            if (!response.IsSuccessStatusCode) {
                return;
            }

            ClearForm();
            await LoadData();
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

        private void ClearForm() {
            selectedResident = null;
            residentNameInput = null;
            selectedGroceryDayID = null;
            selectedPaymentMethodIDs.Clear();
            medicineTimes.Clear();
        }
    }
}