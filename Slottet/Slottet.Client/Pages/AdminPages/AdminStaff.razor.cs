using Microsoft.AspNetCore.Components;
using Slottet.Shared;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminStaff
    {
        [Inject] public HttpClient httpClient { get; set; }

        // ── Staff ─────────────────────────────────────────────────────────
        private List<EditStaffDTO>?       staffMembers;
        private List<DepartmentLookupDTO> departments    = new();
        private string                   staffNameInput  = string.Empty;
        private string                   initialsInput   = string.Empty;
        private string                   roleInput       = string.Empty;
        private Guid                     departmentIdInput;
        private EditStaffDTO?            selectedItem;
        private bool                     isBusy          = false;
        private bool                     loadFailed      = false;

        // ── Særligt Ansvar ────────────────────────────────────────────────
        private List<SpecialResponsibilityEntryDto>? specialResponsibilities;
        private SpecialResponsibilityEntryDto?       selectedSr;
        private string?                              srDescriptionInput;
        private bool                                 srLoadFailed = false;
        private bool                                 srBusy       = false;

        // ── Init ──────────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            await LoadDepartments();
            await Task.WhenAll(LoadStaff(), LoadSr());
        }

        // ═════════════════════════════════════════
        //  STAFF
        // ═════════════════════════════════════════

        private async Task LoadDepartments()
        {
            departments = await httpClient.GetFromJsonAsync<List<DepartmentLookupDTO>>("api/Department")
                ?? new List<DepartmentLookupDTO>();
        }

        private bool HasValidInput =>
            !string.IsNullOrWhiteSpace(staffNameInput) &&
            !string.IsNullOrWhiteSpace(initialsInput)  &&
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
                loadFailed   = true;
            }
        }

        private string GetDepartmentName(Guid departmentId) =>
            departments.FirstOrDefault(x => x.ID == departmentId)?.Name ?? departmentId.ToString();

        private void SelectItem(EditStaffDTO item)
        {
            selectedItem      = item;
            staffNameInput    = item.StaffName;
            initialsInput     = item.Initials;
            roleInput         = item.Role;
            departmentIdInput = item.DepartmentID;
        }

        private async Task Create()
        {
            if (!CanCreate) return;

            isBusy = true;
            try
            {
                var newItem = new EditStaffDTO
                {
                    StaffID      = Guid.NewGuid(),
                    StaffName    = staffNameInput.Trim(),
                    Initials     = initialsInput.Trim(),
                    Role         = roleInput.Trim(),
                    DepartmentID = departmentIdInput
                };

                var response = await httpClient.PostAsJsonAsync("api/Staff", newItem);
                if (response.IsSuccessStatusCode)
                {
                    await LoadStaff();
                    ClearStaffForm();
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
                selectedItem!.StaffName    = staffNameInput.Trim();
                selectedItem.Initials      = initialsInput.Trim();
                selectedItem.Role          = roleInput.Trim();
                selectedItem.DepartmentID  = departmentIdInput;

                var response = await httpClient.PutAsJsonAsync($"api/Staff/{selectedItem.StaffID}", selectedItem);
                if (response.IsSuccessStatusCode)
                {
                    await LoadStaff();
                    ClearStaffForm();
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
                    await LoadStaff();
                    ClearStaffForm();
                }
            }
            finally
            {
                isBusy = false;
            }
        }

        private void ClearStaffForm()
        {
            selectedItem      = null;
            staffNameInput    = string.Empty;
            initialsInput     = string.Empty;
            roleInput         = string.Empty;
            departmentIdInput = Guid.Empty;
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
                srLoadFailed            = true;
            }
        }

        private void SelectSr(SpecialResponsibilityEntryDto item)
        {
            selectedSr         = item;
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
                    Description             = srDescriptionInput
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
            selectedSr         = null;
            srDescriptionInput = string.Empty;
        }
    }
}
