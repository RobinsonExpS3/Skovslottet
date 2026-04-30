using Microsoft.AspNetCore.Components;
using Slottet.Shared;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    public partial class AdminStaff
    {
        [Inject] public HttpClient httpClient { get; set; }

        // ── Staff ─────────────────────────────────────────────────────────
        private List<Staff>? staffMembers;
        private string?      staffNameInput;
        private string?      initialsInput;
        private string?      roleInput;
        private Staff?       selectedItem;
        private bool         loadFailed = false;

        // ── Særligt Ansvar ────────────────────────────────────────────────
        private List<SpecialResponsibilityEntryDto>? specialResponsibilities;
        private SpecialResponsibilityEntryDto?       selectedSr;
        private string?                              srDescriptionInput;
        private string?                              srStaffNameInput;
        private bool                                 srLoadFailed = false;
        private bool                                 srBusy       = false;

        // ── Init ──────────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            await Task.WhenAll(LoadStaff(), LoadSr());
        }

        // ═════════════════════════════════════════
        //  STAFF
        // ═════════════════════════════════════════

        private async Task LoadStaff()
        {
            try
            {
                staffMembers = await httpClient.GetFromJsonAsync<List<Staff>>("api/Staff/Staffs");
            }
            catch
            {
                staffMembers = new List<Staff>();
                loadFailed   = true;
            }
        }

        private void SelectItem(Staff item)
        {
            selectedItem   = item;
            staffNameInput = item.StaffName;
            initialsInput  = item.Initials;
            roleInput      = item.Role;
        }

        private async Task Create()
        {
            var newItem = new Staff
            {
                StaffID   = Guid.NewGuid(),
                StaffName = staffNameInput,
                Initials  = initialsInput,
                Role      = roleInput
            };

            var response = await httpClient.PostAsJsonAsync("api/Staff", newItem);
            if (response.IsSuccessStatusCode)
            {
                await LoadStaff();
                ClearStaffForm();
            }
        }

        private async Task Update()
        {
            if (selectedItem == null) return;

            selectedItem.StaffName = staffNameInput;
            selectedItem.Initials  = initialsInput;
            selectedItem.Role      = roleInput;

            var response = await httpClient.PutAsJsonAsync($"api/Staff/{selectedItem.StaffID}", selectedItem);
            if (response.IsSuccessStatusCode)
            {
                await LoadStaff();
                ClearStaffForm();
            }
        }

        private async Task Delete()
        {
            if (selectedItem == null) return;

            var response = await httpClient.DeleteAsync($"api/Staff/{selectedItem.StaffID}");
            if (response.IsSuccessStatusCode)
            {
                await LoadStaff();
                ClearStaffForm();
            }
        }

        private void ClearStaffForm()
        {
            selectedItem   = null;
            staffNameInput = string.Empty;
            initialsInput  = string.Empty;
            roleInput      = string.Empty;
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
            srStaffNameInput   = item.StaffName;
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
                    Description             = srDescriptionInput,
                    StaffName               = srStaffNameInput ?? string.Empty
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
                selectedSr.StaffName   = srStaffNameInput   ?? string.Empty;

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
            srStaffNameInput   = string.Empty;
        }

        // ── Model ─────────────────────────────────────────────────────────
        public class Staff
        {
            public Guid    StaffID   { get; set; }
            public string  StaffName { get; set; }
            public string  Initials  { get; set; }
            public string  Role      { get; set; }
        }
    }
}
