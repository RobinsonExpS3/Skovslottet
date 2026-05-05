using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminStaff
    {
        [Inject]
        public HttpClient httpClient { get; set; } = default!;

        private List<EditStaffDTO> staffMembers = new();
        private List<DepartmentLookupDTO> departments = new();
        private string staffNameInput = string.Empty;
        private string initialsInput = string.Empty;
        private string roleInput = string.Empty;
        private Guid departmentIdInput;
        private EditStaffDTO? selectedItem;
        private bool isBusy;
        private bool loadFailed;
        private string? loadErrorMessage;

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
                isBusy = false;
                await LoadDepartments();
                await LoadData();
            }

        }

        private async Task LoadDepartments()
        {
            var result = await AdminHttp.GetJsonAsync<List<DepartmentLookupDTO>>(httpClient, "api/Department");
            if (result.Failed)
            {
                departments = new();
                loadFailed = true;
                loadErrorMessage = result.ErrorMessage;
                return;
            }

            departments = result.Value ?? new();
        }

        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(staffNameInput) &&
            !string.IsNullOrWhiteSpace(initialsInput) &&
            !string.IsNullOrWhiteSpace(roleInput) &&
            departmentIdInput != Guid.Empty;

        private bool CanCreate => !loadFailed && !isBusy && HasValidInput;
        private bool CanUpdate => !loadFailed && !isBusy && selectedItem is not null;
        private bool CanDelete => !loadFailed && !isBusy && selectedItem is not null;

        private async Task LoadData()
        {
            if (loadFailed) return;

            var result = await AdminHttp.GetJsonAsync<List<EditStaffDTO>>(httpClient, "api/Staff/Staffs");
            if (result.Failed)
            {
                staffMembers = new();
                loadFailed = true;
                loadErrorMessage = result.ErrorMessage;
                return;
            }

            staffMembers = result.Value ?? new();
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
