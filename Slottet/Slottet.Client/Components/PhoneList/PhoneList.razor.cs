using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Components.PhoneList
{
    public partial class PhoneList
    {
        // ── Parameters ────────────────────────────────────────────────────
        [Parameter, EditorRequired] public List<PhoneEntryDto>          Items    { get; set; } = default!;
        [Parameter]                 public List<string>                  AllStaff { get; set; } = new();
        [Parameter]                 public EventCallback<List<PhoneEntryDto>> OnSave { get; set; }

        // ── State ─────────────────────────────────────────────────────────
        private List<PhoneEntryDto> _editItems = new();
        private bool                _saved;

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override void OnParametersSet()
        {
            _editItems = Items.Select(p => new PhoneEntryDto { Number = p.Number, StaffName = p.StaffName }).ToList();
            _saved     = false;
        }

        // ── Mutations ─────────────────────────────────────────────────────
        private void AddItem()          => _editItems.Add(new PhoneEntryDto());
        private void RemoveItem(int i)  => _editItems.RemoveAt(i);

        private async Task Save()
        {
            Items.Clear();
            Items.AddRange(_editItems.Where(p => !string.IsNullOrWhiteSpace(p.Number)));
            _saved = true;
            await OnSave.InvokeAsync(Items);
        }
    }
}
