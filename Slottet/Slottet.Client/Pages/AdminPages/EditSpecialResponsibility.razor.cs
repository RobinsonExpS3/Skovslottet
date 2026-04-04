using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class EditSpecialResponsibility
    {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<SpecialResponsibility>? specialResponsibilities;
        private string? taskNameInput;
        private SpecialResponsibility? selectedItem;

        private async Task LoadData() {
            specialResponsibilities = await httpClient.GetFromJsonAsync<List<SpecialResponsibility>>(
                "api/SpecialResponsibility/SpecialResponsibilities"
            );
        }

        protected override async Task OnInitializedAsync() {
            await LoadData();
        }

        private void SelectItem(SpecialResponsibility item) {
            selectedItem = item;
            taskNameInput = item.TaskName;
        }

        private async Task Create() {
            var newItem = new SpecialResponsibility {
                SpecialReponsibilityID = Guid.NewGuid(),
                TaskName = taskNameInput
            };

            var response = await httpClient.PostAsJsonAsync("api/SpecialResponsibility", newItem);

            if(response.IsSuccessStatusCode) {
                await LoadData();
                taskNameInput = string.Empty;
            }
        }

        private async Task Update() {
            if(selectedItem == null) {
                return;
            }

            selectedItem.TaskName = taskNameInput;

            var response = await httpClient.PutAsJsonAsync($"api/SpecialResponsibility/{selectedItem.SpecialReponsibilityID}", selectedItem);

            if(response.IsSuccessStatusCode) {
                await LoadData();
                selectedItem = null;
                taskNameInput = string.Empty;
            }
        }

        private async Task Delete() {
            if (selectedItem == null) {
                return;
            }

            var response = await httpClient.DeleteAsync($"api/SpecialResponsibility/{selectedItem.SpecialReponsibilityID}");

            if(response.IsSuccessStatusCode) {
                await LoadData();
                selectedItem = null;
                taskNameInput = string.Empty;
            }

        }

        public class SpecialResponsibility {
            public Guid SpecialReponsibilityID { get; set; }
            public string? TaskName { get; set; }
        }
    }
}