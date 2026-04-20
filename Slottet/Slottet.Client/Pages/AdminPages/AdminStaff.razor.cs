using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminStaff
    {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<Staff>? staffMembers;
        private string? staffNameInput;
        private string? initialsInput;
        private string? roleInput;
        private Staff? selectedItem;
        private bool loadFailed = false;

        private async Task LoadData()
        {
            try
            {
                staffMembers = await httpClient.GetFromJsonAsync<List<Staff>>(
                    "api/Staff/staffMembers"
                );
            }
            catch
            {
                staffMembers = new List<Staff>();
                loadFailed = true;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private void SelectItem(Staff item)
        {
            selectedItem = item;
            staffNameInput = item.StaffName;
            initialsInput = item.Initials;
            roleInput = item.Role;
        }

        private async Task Create()
        {
            var newItem = new Staff
            {
                StaffID = Guid.NewGuid(),
                StaffName = staffNameInput,
                Initials = initialsInput,
                Role = roleInput
            };

            var response = await httpClient.PostAsJsonAsync("api/Staff", newItem);

            if (response.IsSuccessStatusCode)
            {
                await LoadData();
                staffNameInput = string.Empty;
                initialsInput = string.Empty;
                roleInput = string.Empty;
            }
        }

        private async Task Update()
        {
            if (selectedItem == null)
            {
                return;
            }

            selectedItem.StaffName = staffNameInput;
            selectedItem.Initials = initialsInput;
            selectedItem.Role = roleInput;

            var response = await httpClient.PutAsJsonAsync($"api/Staff/{selectedItem.StaffID}", selectedItem);

            if (response.IsSuccessStatusCode)
            {
                await LoadData();
                selectedItem = null;
                staffNameInput = string.Empty;
                initialsInput = string.Empty;
                roleInput = string.Empty;
            }
        }

        private async Task Delete()
        {
            if (selectedItem == null)
            {
                return;
            }

            var response = await httpClient.DeleteAsync($"api/Staff/{selectedItem.StaffID}");

            if (response.IsSuccessStatusCode)
            {
                await LoadData();
                selectedItem = null;
                staffNameInput = string.Empty;
                initialsInput = string.Empty;
                roleInput = string.Empty;
            }

        }

        public class Staff
        {
            public Guid StaffID { get; set; }
            public string StaffName { get; set; }
            public string Initials { get; set; }
            public string Role { get; set; }
        }
        
    }
}