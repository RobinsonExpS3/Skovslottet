using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminResident
    {
        [Inject]
        public HttpClient httpClient { get; set; } = default!;

        private List<EditResidentDTO>? residents;
        private EditResidentDTO? selectedResident;
        private bool loadFailed = false;
        private string? loadErrorMessage;
        private Guid? selectedGroceryDayID;
        private List<Guid> selectedPaymentMethodIDs = new();
        private List<ResidentLookupDTO> paymentMethods = new();
        private List<ResidentLookupDTO> groceryDays = new();
        private List<TimeInput> medicineTimes = new();
        private string? residentNameInput;
        private bool isActiveInput = true;

        private int selectedHour = 8;
        private int selectedMinute = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            loadFailed = false;
            loadErrorMessage = null;

            var residentResult = await AdminHttp.GetJsonAsync<List<EditResidentDTO>>(httpClient, "api/Resident/Residents");
            if (residentResult.Failed)
            {
                residents = new();
                groceryDays = new();
                paymentMethods = new();
                loadFailed = true;
                loadErrorMessage = residentResult.ErrorMessage;
                return;
            }

            var groceryDayResult = await AdminHttp.GetJsonAsync<List<ResidentLookupDTO>>(httpClient, "api/Resident/groceryDays");
            var paymentMethodResult = await AdminHttp.GetJsonAsync<List<ResidentLookupDTO>>(httpClient, "api/Resident/paymentMethods");

            if (groceryDayResult.Failed || paymentMethodResult.Failed)
            {
                residents = new();
                groceryDays = new();
                paymentMethods = new();
                loadFailed = true;
                loadErrorMessage = groceryDayResult.ErrorMessage ?? paymentMethodResult.ErrorMessage;
                return;
            }

            residents = residentResult.Value ?? new();
            groceryDays = groceryDayResult.Value ?? new();
            paymentMethods = paymentMethodResult.Value ?? new();
        }

        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(residentNameInput)
            && selectedGroceryDayID != null
            && selectedResident == null;

        private bool CanCreate => !isActiveInput && HasValidInput;
        private bool CanUpdate => !isActiveInput && selectedResident != null;
        private bool CanDelete => !isActiveInput && selectedResident != null;

        private async Task SelectResidentAsync(EditResidentDTO resident)
        {
            selectedResident = resident;
            residentNameInput = resident.ResidentName;
            selectedGroceryDayID = resident.GroceryDayID;
            isActiveInput = resident.IsActive;

            var result = await AdminHttp.GetJsonAsync<EditResidentDTO>(httpClient, $"api/Resident/{resident.ResidentID}");
            if (result.Failed)
            {
                loadFailed = true;
                loadErrorMessage = result.ErrorMessage;
                selectedPaymentMethodIDs = new();
                medicineTimes = new();
                return;
            }

            var dto = result.Value;
            selectedPaymentMethodIDs = dto?.PaymentMethodIDs ?? new();
            medicineTimes = dto?.MedicineTimes
                .Select(t => new TimeInput { Time = TimeOnly.FromDateTime(t) })
                .ToList() ?? new();
        }

        private async Task CreateAsync()
        {
            try
            {
                var dto = new EditResidentDTO
                {
                    ResidentName = residentNameInput ?? string.Empty,
                    GroceryDayID = selectedGroceryDayID ?? Guid.Empty,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                    IsActive = isActiveInput
                };

                var response = await httpClient.PostAsJsonAsync("api/Resident", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                ClearForm();
                await LoadDataAsync();

            }
            finally
            {
                isActiveInput = false;
            }
        }



        private async Task UpdateAsync()
        {
            if (selectedResident == null) return;

            if (!CanUpdate) return;
            isActiveInput = true;

            try
            {

                var dto = new EditResidentDTO
                {
                    ResidentID = selectedResident.ResidentID,
                    ResidentName = residentNameInput ?? string.Empty,
                    GroceryDayID = selectedGroceryDayID ?? Guid.Empty,
                    PaymentMethodIDs = selectedPaymentMethodIDs.ToList(),
                    MedicineTimes = medicineTimes.Select(t => DateTime.Today.Add(t.Time.ToTimeSpan())).ToList(),
                    IsActive = isActiveInput
                };

                var response = await httpClient.PutAsJsonAsync($"api/Resident/{selectedResident.ResidentID}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            finally
            {
                isActiveInput = false;
            }


            ClearForm();
            await LoadDataAsync();
        }


        private async Task DeleteAsync()
        {
            if (selectedResident == null) return;

            if (!CanDelete) return;
            isActiveInput = true;

            try {
                var response = await httpClient.DeleteAsync($"api/Resident/{selectedResident.ResidentID}");

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                ClearForm();
                await LoadDataAsync();
            }
            finally
            {
                isActiveInput = false;
            }
        }

        private void OnPaymentMethodsChanged(ChangeEventArgs e)
        {
            selectedPaymentMethodIDs.Clear();

            if (e.Value is string[] valuesArray)
            {
                foreach (var v in valuesArray)
                    if (Guid.TryParse(v, out var id))
                        selectedPaymentMethodIDs.Add(id);
            }
            else if (e.Value is string singleValue && Guid.TryParse(singleValue, out var id))
            {
                selectedPaymentMethodIDs.Add(id);
            }
        }

        public class TimeInput
        {
            public TimeOnly Time { get; set; } = new(8, 0);
        }

        private void RemoveMedicineTime(TimeInput time)
        {
            medicineTimes.Remove(time);
        }

        private void AddMedicineTime()
        {
            var time = new TimeOnly(selectedHour, selectedMinute);

            if (!medicineTimes.Any(t => t.Time == time))
            {
                medicineTimes.Add(new TimeInput { Time = time });
            }
        }

        private string GetGroceryDayName(Guid groceryDayId)
        {
            return groceryDays.FirstOrDefault(day => day.ID == groceryDayId)?.Name ?? "Ukendt dag";
        }

        private void ClearForm()
        {
            selectedResident = null;
            residentNameInput = null;
            selectedGroceryDayID = null;
            selectedPaymentMethodIDs.Clear();
            medicineTimes.Clear();
            isActiveInput = true;
        }
    }
}
