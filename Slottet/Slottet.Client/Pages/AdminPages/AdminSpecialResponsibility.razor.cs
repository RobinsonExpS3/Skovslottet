using Microsoft.AspNetCore.Components;
using Slottet.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminSpecialResponsibility
    {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<SpecialResponsibilityEntryDto>? specialResponsibilities;
        private string? taskNameInput;
        private SpecialResponsibilityEntryDto? selectedItem;
        private bool loadFailed = false;
        private bool _isBusy;

        private async Task LoadData() {
            try {
                specialResponsibilities = await httpClient.GetFromJsonAsync<List<SpecialResponsibilityEntryDto>>(
                    "api/SpecialResponsibility/SpecialResponsibilities"
                );
            } catch {
                specialResponsibilities = new List<SpecialResponsibilityEntryDto>();
                loadFailed = true;
            }
        }

        protected override async Task OnInitializedAsync() {
            await LoadData();
        }

        private bool HasValidInput => 
            !string.IsNullOrWhiteSpace(taskNameInput)
            && selectedItem == null;

        private bool CanCreate => !_isBusy && HasValidInput;
        private bool CanUpdate => !_isBusy && selectedItem != null;
        private bool CanDelete => !_isBusy && selectedItem != null;

        private void SelectItem(SpecialResponsibilityEntryDto item) {
            selectedItem = item;
            taskNameInput = item.Description;
        }

        private async Task CreateAsync() {
            if(!CanCreate) return;
            _isBusy = true;
            
            try {
                var newItem = new SpecialResponsibilityEntryDto {
                    SpecialResponsibilityID = Guid.NewGuid(),
                    Description = taskNameInput
                };

                var response = await httpClient.PostAsJsonAsync("api/SpecialResponsibility", newItem);

                if (response.IsSuccessStatusCode) {
                    await LoadData();
                    taskNameInput = string.Empty;
                }
            } finally {
                _isBusy = false;
            }
        }

        private async Task UpdateAsync() {
            if(selectedItem == null) {
                return;
            }

            if(!CanUpdate) return;
            _isBusy = true;

            try {
                selectedItem.Description = taskNameInput;

                var response = await httpClient.PutAsJsonAsync($"api/SpecialResponsibility/{selectedItem.SpecialResponsibilityID}", selectedItem);

                if (response.IsSuccessStatusCode) {
                    await LoadData();
                    selectedItem = null;
                    taskNameInput = string.Empty;
                }
            } finally {
                _isBusy = false; 
            }
        }

        private async Task DeleteAsync() {
            if (selectedItem == null) {
                return;
            }

            if(!CanDelete) return;
            _isBusy = true;

            try {
                var response = await httpClient.DeleteAsync($"api/SpecialResponsibility/{selectedItem.SpecialResponsibilityID}");

                if (response.IsSuccessStatusCode) {
                    await LoadData();
                    selectedItem = null;
                    taskNameInput = string.Empty;
                }
            } finally {
                _isBusy = false; 
            }
        }
    }
}