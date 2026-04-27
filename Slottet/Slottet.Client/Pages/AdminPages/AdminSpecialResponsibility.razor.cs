using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminSpecialResponsibility
    {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<SpecialResponsibility>? specialResponsibilities;
        private string? taskNameInput;
        private SpecialResponsibility? selectedItem;
        private bool loadFailed = false;
        private bool _isBusy;

        private async Task LoadData() {
            try {
                specialResponsibilities = await httpClient.GetFromJsonAsync<List<SpecialResponsibility>>(
                    "api/SpecialResponsibility/SpecialResponsibilities"
                );
            } catch {
                specialResponsibilities = new List<SpecialResponsibility>();
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

        private void SelectItem(SpecialResponsibility item) {
            selectedItem = item;
            taskNameInput = item.TaskName;
        }

        private async Task CreateAsync() {
            if(!CanCreate) return;
            _isBusy = true;
            
            try {
                var newItem = new SpecialResponsibility {
                    SpecialResponsibilityID = Guid.NewGuid(),
                    TaskName = taskNameInput
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
                selectedItem.TaskName = taskNameInput;

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

        public class SpecialResponsibility {
            public Guid SpecialResponsibilityID { get; set; }
            public string? TaskName { get; set; }
            public Guid ShiftBoardID { get; set; }
        }
    }
}