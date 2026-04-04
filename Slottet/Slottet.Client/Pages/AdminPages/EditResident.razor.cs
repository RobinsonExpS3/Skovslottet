using System.Net.Http;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class EditResident
    {
        private List<Resident>? residents;
        private Resident? selectedResident;

        private async Task LoadData() {
            residents = await httpClient.GetFromJsonAsync<List<Resident>>(
                "api/Resident/Residents"
            );
        }

        protected override async Task OnInitializedAsync() {
            await LoadData();
        }
    }
}