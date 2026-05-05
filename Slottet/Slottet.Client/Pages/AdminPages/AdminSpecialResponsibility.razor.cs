using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminSpecialResponsibility
    {
        [Inject]
        public HttpClient httpClient { get; set; } = default!;

        private List<SpecialResponsibilityEntryDto>? specialResponsibilities;
        private string? taskNameInput;
        private SpecialResponsibilityEntryDto? selectedItem;
        private bool loadFailed = false;
        private string? loadErrorMessage;
        private bool _isBusy;


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
                _isBusy = false;
                await LoadData();
            }
        }
        private async Task LoadData()
        {
            loadFailed = false;
            loadErrorMessage = null;

            var result = await AdminHttp.GetJsonAsync<List<SpecialResponsibilityEntryDto>>(
                httpClient,
                "api/SpecialResponsibility/SpecialResponsibilities"
            );

            if (result.Failed)
            {
                specialResponsibilities = new List<SpecialResponsibilityEntryDto>();
                loadFailed = true;
                loadErrorMessage = result.ErrorMessage;
                return;
            }

            specialResponsibilities = result.Value ?? new List<SpecialResponsibilityEntryDto>();
        }

        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(taskNameInput)
            && selectedItem == null;

        private bool CanCreate => !loadFailed && !_isBusy && HasValidInput;
        private bool CanUpdate => !loadFailed && !_isBusy && selectedItem != null;
        private bool CanDelete => !loadFailed && !_isBusy && selectedItem != null;

        private void SelectItem(SpecialResponsibilityEntryDto item)
        {
            selectedItem = item;
            taskNameInput = item.Description;
        }

        private async Task CreateAsync()
        {
            if (!CanCreate) return;
            _isBusy = true;

            try
            {
                var newItem = new SpecialResponsibilityEntryDto
                {
                    SpecialResponsibilityID = Guid.NewGuid(),
                    Description = taskNameInput ?? string.Empty
                };

                var response = await httpClient.PostAsJsonAsync("api/SpecialResponsibility", newItem);

                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    taskNameInput = string.Empty;
                }
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async Task UpdateAsync()
        {
            if (selectedItem == null)
            {
                return;
            }

            if (!CanUpdate) return;
            _isBusy = true;

            try
            {
                selectedItem.Description = taskNameInput ?? string.Empty;

                var response = await httpClient.PutAsJsonAsync($"api/SpecialResponsibility/{selectedItem.SpecialResponsibilityID}", selectedItem);

                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    selectedItem = null;
                    taskNameInput = string.Empty;
                }
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async Task DeleteAsync()
        {
            if (selectedItem == null)
            {
                return;
            }

            if (!CanDelete) return;
            _isBusy = true;

            try
            {
                var response = await httpClient.DeleteAsync($"api/SpecialResponsibility/{selectedItem.SpecialResponsibilityID}");

                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    selectedItem = null;
                    taskNameInput = string.Empty;
                }
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}
