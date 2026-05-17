using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Components.SpecialResponsibility
{
    public partial class SpecialResponsibility
    {
        // ── DI ────────────────────────────────────────────────────────────
        [Inject] private HttpClient Http { get; set; } = default!;

        // ── Parameters ────────────────────────────────────────────────────
        [Parameter, EditorRequired] public List<SpecialResponsibilityEntryDto>               Items         { get; set; } = default!;
        [Parameter]                 public List<string>                                      AllStaff      { get; set; } = new();
        [Parameter]                 public Guid                                              ShiftBoardID  { get; set; }
        [Parameter]                 public EventCallback<List<SpecialResponsibilityEntryDto>> OnSave       { get; set; }

        // ── State ─────────────────────────────────────────────────────────
        private List<SpecialResponsibilityEntryDto> _editItems = new();
        private bool                                _saved;

        protected override void OnParametersSet()
        {
            _editItems = Items.Select(s => new SpecialResponsibilityEntryDto
            {
                SpecialResponsibilityID = s.SpecialResponsibilityID,
                DepartmentID            = s.DepartmentID,
                Description             = s.Description,
                StaffName               = s.StaffName,
                StaffInitials           = s.StaffInitials,
            }).ToList();
            _saved = false;
        }

        // ── List mutations ────────────────────────────────────────────────
        private void RemoveItem(int i) => _editItems.RemoveAt(i);

        // ── Save ──────────────────────────────────────────────────────────
        private async Task Save()
        {
            if (ShiftBoardID != Guid.Empty)
            {
                foreach (var item in _editItems.Where(s => !string.IsNullOrWhiteSpace(s.Description)))
                {
                    var dto = new SpecialResponsibilityAssignmentDto
                    {
                        SpecialResponsibilityID = item.SpecialResponsibilityID,
                        ShiftBoardID            = ShiftBoardID,
                        DepartmentID            = item.DepartmentID,
                        StaffName               = item.StaffName ?? string.Empty,
                    };
                    await Http.PatchAsJsonAsync("api/shiftboard/special-responsibility", dto);
                }
            }

            Items.Clear();
            Items.AddRange(_editItems.Where(s => !string.IsNullOrWhiteSpace(s.Description)));
            _saved = true;
            await OnSave.InvokeAsync(Items);
        }
    }
}
