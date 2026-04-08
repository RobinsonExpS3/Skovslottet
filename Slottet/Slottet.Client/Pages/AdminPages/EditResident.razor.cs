using Microsoft.AspNetCore.Components;
using Slottet.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class EditResident {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<ResidentViewModel>? residents;
        private ResidentViewModel? selectedResident;
        private bool loadFailed = false;
        private Guid selectedGroceryDayID;
        private List<string> selectedPaymentMethodIDs = new();
        private List<PaymentMethodDTO> paymentMethods = new ();
        private List<GroceryDayDTO> groceryDays = new();
        private List<TimeInput> medicineTimes = new();
        private string? residentNameInput;

        private async Task LoadData() {
            try {
                residents = await httpClient.GetFromJsonAsync<List<ResidentViewModel>>(
                    "api/Resident/Residents"
                );

                groceryDays = await httpClient.GetFromJsonAsync<List<GroceryDayDTO>>(
                    "api/GroceryDay"
                );

                paymentMethods = await httpClient.GetFromJsonAsync<List<PaymentMethodDTO>>(
                    "api/PaymentMethod"
                );

            } catch {
                residents = new List<ResidentViewModel>();
                groceryDays = new List<GroceryDayDTO>();
                paymentMethods = new List<PaymentMethodDTO>();
                loadFailed = true;
            }
        }

        protected override async Task OnInitializedAsync() {
            await LoadData();
        }

        private void SelectResident(ResidentViewModel resident) {
            selectedResident = resident;
        }

        private async Task Create() {
            var dto = new ResidentDTO {
                ResidentName = residentNameInput,
                GroceryDayID = selectedGroceryDayID,
                PaymentMethodIDs = selectedPaymentMethodIDs.Select(id => Guid.Parse(id)).ToList(),
                MedicineTimes = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                IsActive = true
            };

            var response = await httpClient.PostAsJsonAsync("api/Resident", dto);

            if(response.IsSuccessStatusCode) {
                await LoadData();
            }
        }

        private async Task Update() {
            if(selectedResident == null) {
                return;
            }

            var dto = new ResidentDTO {
                ResidentName = residentNameInput,
                GroceryDayID = selectedGroceryDayID,
                PaymentMethodIDs = selectedPaymentMethodIDs.Select(id => Guid.Parse(id)).ToList(),
                MedicineTimes = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                IsActive = selectedResident.IsActive
            };

            await httpClient.PutAsJsonAsync(
                $"api/Resident/{selectedResident.ResidentID}", dto
            );

            await LoadData();
        }

        private async Task Delete() {
            if(selectedResident == null) { 
                return; 
            }

            await httpClient.DeleteAsync($"api/Resident/{selectedResident.ResidentID}");

            selectedResident = null;
            await LoadData();
        }

        private void OnPaymentMethodsChanged(ChangeEventArgs e) {
            selectedPaymentMethodIDs.Clear();

            if (e.Value is string[] valuesArray) {
                selectedPaymentMethodIDs = valuesArray.ToList();
            } else if (e.Value is string singleValue) {
                selectedPaymentMethodIDs.Add(singleValue);
            }
        }

        public class TimeInput {
            public TimeOnly Time { get; set; } = new(8, 0);
        }

        private void AddMedicineTime() {
            medicineTimes.Add(new TimeInput());
        }
    }
}