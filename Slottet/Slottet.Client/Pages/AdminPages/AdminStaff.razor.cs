using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminStaff
    {
        [Inject]
        public HttpClient httpClient { get; set; }

        private List<EditStaffDTO> staffMembers = new();
        private List<DepartmentLookupDTO> departments = new();
        private string staffNameInput = string.Empty;
        private string initialsInput = string.Empty;
        private string roleInput = string.Empty;
        private Guid departmentIdInput;
        private EditStaffDTO? selectedItem;
        private bool isBusy;

        protected override async Task OnInitializedAsync()
        {
            await LoadDepartments();
            await LoadData();
        }

        private async Task LoadDepartments()
        {
            departments = await httpClient.GetFromJsonAsync<List<DepartmentLookupDTO>>("api/Department")
                ?? new List<DepartmentLookupDTO>();
        }

        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(staffNameInput) &&
            !string.IsNullOrWhiteSpace(initialsInput) &&
            !string.IsNullOrWhiteSpace(roleInput) &&
            departmentIdInput != Guid.Empty;

        private bool CanCreate => !isBusy && HasValidInput;
        private bool CanUpdate => !isBusy && selectedItem is not null;
        private bool CanDelete => !isBusy && selectedItem is not null;

        private async Task LoadData()
        {
            staffMembers = await httpClient.GetFromJsonAsync<List<EditStaffDTO>>("api/Staff/Staffs")
                ?? new List<EditStaffDTO>();
        }

        private void SelectItem(EditStaffDTO item)
        {
            selectedItem = item;
            staffNameInput = item.StaffName;
            initialsInput = item.Initials;
            roleInput = item.Role;
            departmentIdInput = item.DepartmentID;
        }

        private string GetDepartmentName(Guid departmentId) =>
            departments.FirstOrDefault(x => x.ID == departmentId)?.Name ?? departmentId.ToString();

        private async Task Create()
        {
            if (!CanCreate) return;

            isBusy = true;
            try
            {
                var newItem = new EditStaffDTO
                {
                    StaffID = Guid.NewGuid(),
                    StaffName = staffNameInput.Trim(),
                    Initials = initialsInput.Trim(),
                    Role = roleInput.Trim(),
                    DepartmentID = departmentIdInput
                };

                var response = await httpClient.PostAsJsonAsync("api/Staff", newItem);
                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    ClearForm();
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
                selectedItem!.StaffName = staffNameInput.Trim();
                selectedItem.Initials = initialsInput.Trim();
                selectedItem.Role = roleInput.Trim();
                selectedItem.DepartmentID = departmentIdInput;

                var response = await httpClient.PutAsJsonAsync($"api/Staff/{selectedItem.StaffID}", selectedItem);
                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    ClearForm();
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
                var response = await httpClient.DeleteAsync($"api/Staff/{selectedItem!.StaffID}");
                if (response.IsSuccessStatusCode)
                {
                    await LoadData();
                    ClearForm();
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
            departmentIdInput = Guid.Empty;
        }
    }
}
