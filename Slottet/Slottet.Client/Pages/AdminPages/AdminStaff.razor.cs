using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminStaff
    {
        [Inject]
        public HttpClient httpClient { get; set; }
        private string? loadErrorMessage;
        private string? passwordInput;
        private bool isAdminChecked = false;

        // ── Staff ─────────────────────────────────────────────────────────
        private List<EditStaffDTO>? staffMembers;
        private List<DepartmentLookupDTO> departments = new();
        private string staffNameInput = string.Empty;
        private string initialsInput = string.Empty;
        private string roleInput = string.Empty;
        private Guid departmentIdInput;
        private EditStaffDTO? selectedItem;
        private bool isBusy = false;
        private bool loadFailed = false;

        // ── Særligt Ansvar ────────────────────────────────────────────────
        private List<SpecialResponsibilityEntryDto>? specialResponsibilities;
        private SpecialResponsibilityEntryDto? selectedSr;
        private string? srDescriptionInput;
        private bool srLoadFailed = false;
        private bool srBusy = false;

        // ── Init ──────────────────────────────────────────────────────────
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
                await Task.WhenAll(LoadStaff(), LoadSr());
            }
        }

        // ═════════════════════════════════════════
        //  STAFF
        // ═════════════════════════════════════════

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
            !string.IsNullOrWhiteSpace(roleInput);

        private bool CanCreate => !isBusy && HasValidInput && selectedItem is null;
        private bool CanUpdate => !isBusy && selectedItem is not null;
        private bool CanDelete => !isBusy && selectedItem is not null;

        private async Task LoadStaff()
        {
            try
            {
                staffMembers = await httpClient.GetFromJsonAsync<List<EditStaffDTO>>("api/Staff/Staffs");
            }
            catch
            {
                staffMembers = new List<EditStaffDTO>();
                loadFailed = true;
            }
        }

        private string GetDepartmentName(Guid departmentId) =>
            departments.FirstOrDefault(x => x.ID == departmentId)?.Name ?? departmentId.ToString();

        private Guid GetDepartmentSkoven() =>
            departments.FirstOrDefault(x =>
                string.Equals(x.Name, "Skoven", StringComparison.OrdinalIgnoreCase))?.ID ?? Guid.Empty;

        private void SelectItem(EditStaffDTO item)
        {
            selectedItem = item;
            staffNameInput = item.StaffName;
            initialsInput = item.Initials;
            roleInput = item.Role;
            departmentIdInput = item.DepartmentID;
        }

        private async Task Create()
        {
            if (!CanCreate) return;

            isBusy = true;
            try
            {
                var staffid = Guid.NewGuid();

                var newItem = new EditStaffDTO
                {
                    StaffID = staffid,
                    StaffName = staffNameInput.Trim(),
                    Initials = initialsInput.Trim(),
                    Role = roleInput.Trim(),
                    DepartmentID = GetDepartmentSkoven(),
                };

                var staffResponse = await httpClient.PostAsJsonAsync("api/Staff", newItem);

                if (!staffResponse.IsSuccessStatusCode)
                {
                    loadErrorMessage = "Kunne ikke oprette medarbejder. Prøv venligst igen senere.";
                    return;
                }



                if (roleInput == "admin")
                {
                    isAdminChecked = true;
                }

                var newUser = new CreateUserForStaffDTO
                {
                    StaffID = staffid,
                    UserName = initialsInput.Trim(),
                    Password = passwordInput,
                    AuthRole = isAdminChecked ? "Admin" : "Employee"
                };

                var userResponse = await httpClient.PostAsJsonAsync("api/Auth/createUserForStaff", newUser);

                if (!userResponse.IsSuccessStatusCode)
                {
                    loadErrorMessage = "Medarbejderen blev oprettet, men login kunne ikke oprettes.";
                    return;
                }

                if (userResponse.IsSuccessStatusCode)
                {
                    await LoadStaff();
                }
            }
            finally
            {
                ClearStaffForm();
                isBusy = false;
            }
        }

        private async Task CreateStaffUser()
        {

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

                if (!response.IsSuccessStatusCode)
                {
                    loadErrorMessage = "Kunne ikke opdatere medarbejder. Prøv venligst igen senere.";
                    return;
                }

                if (roleInput == "admin")
                {
                    isAdminChecked = true;
                }

                var userUpdate = new CreateUserForStaffDTO
                {
                    StaffID = selectedItem.StaffID,
                    UserName = initialsInput.Trim(),
                    Password = passwordInput,
                    AuthRole = isAdminChecked ? "Admin" : "Employee"
                };

                var userResponse = await httpClient.PutAsJsonAsync($"api/Auth/{selectedItem.StaffID}", userUpdate);

                if (userResponse.IsSuccessStatusCode)
                {
                    await LoadStaff();
                }
            }
            finally
            {
                ClearStaffForm();
                isBusy = false;
            }
        }

        private async Task Delete()
        {
            if (!CanDelete) return;

            isBusy = true;
            try
            {
                var userResponse = await httpClient.DeleteAsync($"api/Auth/{selectedItem!.StaffID}");

                var response = await httpClient.DeleteAsync($"api/Staff/{selectedItem!.StaffID}");

                if (response.IsSuccessStatusCode && userResponse.IsSuccessStatusCode)
                {
                    await LoadStaff();
                }
            }
            finally
            {
                ClearStaffForm();
                isBusy = false;
            }
        }

        private void ClearAll()
        {
            ClearStaffForm();
            ClearSrForm();
        }

        private void ClearStaffForm()
        {
            selectedItem = null;
            staffNameInput = string.Empty;
            initialsInput = string.Empty;
            roleInput = string.Empty;
            departmentIdInput = Guid.Empty;
            passwordInput = string.Empty;
        }

        // ═════════════════════════════════════════
        //  SÆRLIGT ANSVAR
        // ═════════════════════════════════════════

        private async Task LoadSr()
        {
            try
            {
                specialResponsibilities = await httpClient.GetFromJsonAsync<List<SpecialResponsibilityEntryDto>>(
                    "api/SpecialResponsibility/SpecialResponsibilities"
                );
            }
            catch
            {
                specialResponsibilities = new List<SpecialResponsibilityEntryDto>();
                srLoadFailed = true;
            }
        }

        private void SelectSr(SpecialResponsibilityEntryDto item)
        {
            selectedSr = item;
            srDescriptionInput = item.Description;
        }

        private async Task CreateSr()
        {
            if (string.IsNullOrWhiteSpace(srDescriptionInput) || srBusy) return;
            srBusy = true;
            try
            {
                var newItem = new SpecialResponsibilityEntryDto
                {
                    SpecialResponsibilityID = Guid.NewGuid(),
                    Description = srDescriptionInput
                };
                var response = await httpClient.PostAsJsonAsync("api/SpecialResponsibility", newItem);
                if (response.IsSuccessStatusCode) { await LoadSr(); ClearSrForm(); }
            }
            finally { srBusy = false; }
        }

        private async Task UpdateSr()
        {
            if (selectedSr == null || srBusy) return;
            srBusy = true;
            try
            {
                selectedSr.Description = srDescriptionInput ?? string.Empty;

                var response = await httpClient.PutAsJsonAsync(
                    $"api/SpecialResponsibility/{selectedSr.SpecialResponsibilityID}", selectedSr);
                if (response.IsSuccessStatusCode) { await LoadSr(); ClearSrForm(); }
            }
            finally { srBusy = false; }
        }

        private async Task DeleteSr()
        {
            if (selectedSr == null || srBusy) return;
            srBusy = true;
            try
            {
                var response = await httpClient.DeleteAsync(
                    $"api/SpecialResponsibility/{selectedSr.SpecialResponsibilityID}");
                if (response.IsSuccessStatusCode) { await LoadSr(); ClearSrForm(); }
            }
            finally { srBusy = false; }
        }

        private void ClearSrForm()
        {
            selectedSr = null;
            srDescriptionInput = string.Empty;
        }
    }
}
