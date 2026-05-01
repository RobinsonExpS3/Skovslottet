using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Components.SpecialResponsibility
{
    public partial class SpecialResponsibility
    {
        // ── Parameters ────────────────────────────────────────────────────
        [Parameter, EditorRequired] public List<SpecialResponsibilityEntryDto>             Items    { get; set; } = default!;
        [Parameter]                 public List<string>                                    AllStaff { get; set; } = new();
        [Parameter]                 public EventCallback<List<SpecialResponsibilityEntryDto>> OnSave { get; set; }

        // ── State ─────────────────────────────────────────────────────────
        private List<SpecialResponsibilityEntryDto> _editItems = new();
        private bool                                _saved;

        protected override void OnParametersSet()
        {
            _editItems = Items.Select(s => new SpecialResponsibilityEntryDto { Description = s.Description, StaffName = s.StaffName }).ToList();
            _saved     = false;
        }

        // ── List mutations ────────────────────────────────────────────────
        private void AddItem()         => _editItems.Add(new SpecialResponsibilityEntryDto());
        private void RemoveItem(int i) => _editItems.RemoveAt(i);

        // ── Save ──────────────────────────────────────────────────────────
        private async Task Save()
        {
            Items.Clear();
            Items.AddRange(_editItems.Where(s => !string.IsNullOrWhiteSpace(s.Description)));
            _saved = true;
            await OnSave.InvokeAsync(Items);
        }
    }
}
