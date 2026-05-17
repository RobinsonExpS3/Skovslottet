using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Components.PhoneList
{
    public partial class PhoneList
    {
        // ── DI ────────────────────────────────────────────────────────────
        [Inject] private HttpClient Http { get; set; } = default!;

        // ── Parameters ────────────────────────────────────────────────────
        [Parameter, EditorRequired] public List<PhoneEntryDto>               Items        { get; set; } = default!;
        [Parameter]                 public List<string>                       AllStaff     { get; set; } = new();
        [Parameter]                 public Guid                               ShiftBoardID { get; set; }
        [Parameter]                 public EventCallback<List<PhoneEntryDto>> OnSave       { get; set; }

        // ── State ─────────────────────────────────────────────────────────
        private List<PhoneEntryDto> _editItems = new();
        private bool                _saved;

        // ── Lifecycle ─────────────────────────────────────────────────────
        protected override void OnParametersSet()
        {
            _editItems = Items.Select(p => new PhoneEntryDto
            {
                PhoneID   = p.PhoneID,
                Number    = p.Number,
                StaffName = p.StaffName,
            }).ToList();
            _saved = false;
        }

        // ── Mutations ─────────────────────────────────────────────────────
        private void AddItem()         => _editItems.Add(new PhoneEntryDto());
        private void RemoveItem(int i) => _editItems.RemoveAt(i);

        private async Task Save()
        {
            if (ShiftBoardID != Guid.Empty)
            {
                foreach (var item in _editItems.Where(p => p.PhoneID != Guid.Empty))
                {
                    var dto = new SwapPhoneDTO
                    {
                        PhoneID      = item.PhoneID,
                        ShiftBoardID = ShiftBoardID,
                        StaffName    = item.StaffName ?? string.Empty,
                    };
                    await Http.PatchAsJsonAsync("api/shiftboard/phone-assignment", dto);
                }
            }

            Items.Clear();
            Items.AddRange(_editItems.Where(p => !string.IsNullOrWhiteSpace(p.Number)));
            _saved = true;
            await OnSave.InvokeAsync(Items);
        }
    }
}
