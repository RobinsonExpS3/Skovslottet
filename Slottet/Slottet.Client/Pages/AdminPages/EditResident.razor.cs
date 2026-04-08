using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class EditResident {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<Resident>? residents;
        private Resident? selectedResident;
        private bool loadFailed = false;

        private string? residentNameInput;

        private async Task LoadData() {
            try {
                residents = await httpClient.GetFromJsonAsync<List<Resident>>(
                    "api/Resident/Residents"
                );
            } catch {
                residents = new List<Resident>();
                loadFailed = true;
            }
        }

        protected override async Task OnInitializedAsync() {
            await LoadData();
        }

        private void SelectResident(Resident resident) {
            selectedResident = resident;
        }

        private async Task Create() {

        }

        private async Task Update() {

        }

        private async Task Delete() {

        }

        public class Resident {

        }
    }
}