using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminStaff
    {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<Staff> staffMembers = new();
        private string staffNameInput = string.Empty;
        private string initialsInput = string.Empty;
        private string roleInput = string.Empty;
        private string errorMessage = string.Empty;
        private Staff? selectedItem;
        private bool isBusy;

        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(staffNameInput) &&
            !string.IsNullOrWhiteSpace(initialsInput) &&
            !string.IsNullOrWhiteSpace(roleInput);

        private bool CanCreate => !isBusy && HasValidInput;
        private bool CanUpdate => !isBusy && selectedItem is not null;
        private bool CanDelete => !isBusy && selectedItem is not null;

        private async Task LoadData()
        {
            try
            {
                staffMembers = await httpClient.GetFromJsonAsync<List<Staff>>(
                    "api/Staff/Staffs"
                )
                ?? new List<Staff>();
                errorMessage = string.Empty;
            }
            catch
            {
                staffMembers = new List<Staff>();
                errorMessage = "Kunne ikke hente personale.";
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
            if (!CanCreate) return;
            isBusy = true;
            try
            {
                errorMessage = string.Empty;
                var newItem = new Staff
                {
                    StaffID = Guid.NewGuid(),
                    StaffName = staffNameInput.Trim(),
                    Initials = initialsInput.Trim(),
                    Role = roleInput.Trim(),
                    DepartmentID = selectedItem?.DepartmentID ?? Guid.Empty
                };

                var response = await httpClient.PostAsJsonAsync("api/Staff", newItem);
                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    ClearForm();
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            finally
            {
                isBusy = false;
            }
        }

        private async Task Update()
        {
            if (!CanUpdate) return;
            isBusy = true;
            try
            {
                errorMessage = string.Empty;
                selectedItem!.StaffName = staffNameInput.Trim();
                selectedItem.Initials = initialsInput.Trim();
                selectedItem.Role = roleInput.Trim();

                var response = await httpClient.PutAsJsonAsync($"api/Staff/{selectedItem.StaffID}", selectedItem);
                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    ClearForm();
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            finally
            {
                isBusy = false;
            }
        }

        private async Task Delete()
        {
            if (!CanDelete) return;
            isBusy = true;
            try
            {
                errorMessage = string.Empty;
                var response = await httpClient.DeleteAsync($"api/Staff/{selectedItem!.StaffID}");
                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    ClearForm();
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            finally
            {
                isBusy = false;
            }
        }

        private void ClearForm()
        {
            selectedItem = null;
            staffNameInput = string.Empty;
            initialsInput = string.Empty;
            roleInput = string.Empty;
        }

        public class Staff
        {
            public Guid StaffID { get; set; }
            public string StaffName { get; set; } = string.Empty;
            public string Initials { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public Guid DepartmentID { get; set; }
        }
        
    }
}
