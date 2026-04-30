using Microsoft.AspNetCore.Components;
using Slottet.Shared;

namespace Slottet.Client.Components.ResidentCard
{
    public partial class ResidentCard
    {
        // ── Parameters ────────────────────────────────────────────────────
        [Parameter, EditorRequired] public ResidentCardDto Resident { get; set; } = default!;
        [Parameter]                 public List<string>    AllStaff { get; set; } = new();
        [Parameter]                 public EventCallback   OnSaved  { get; set; }

        // ── Private PN row wrapper ────────────────────────────────────────
        private class PnRow
        {
            public PNEntryDto  Entry     { get; set; } = new();
            public bool        IsEditing { get; set; }
            public PNEntryDto? Snapshot  { get; set; }
        }

        // ── Static option lists ───────────────────────────────────────────
        public static readonly IReadOnlyList<string> GroceryDays =
        [
            "Mandag", "Tirsdag", "Onsdag", "Torsdag",
            "Fredag", "Lørdag", "Søndag", "Handler selv"
        ];

        // ── Draft state ───────────────────────────────────────────────────
        private ResidentCardDto _draft               = default!;
        private List<PnRow>     _pnRows              = new();
        private bool            _showNewMedicineForm = false;
        private string          _newMedicineLabel    = string.Empty;
        private bool            _showStaffPicker     = false;

        protected override void OnParametersSet()
        {
            _draft  = DeepCopy(Resident);
            _pnRows = _draft.PNEntries
                .OrderBy(p => ParseTime(p.TimeOfAdministration))
                .Select(p => new PnRow { Entry = p, IsEditing = false, Snapshot = ClonePN(p) })
                .ToList();

            _showNewMedicineForm = false;
            _newMedicineLabel    = string.Empty;
            _showStaffPicker     = false;
        }

        // ── Medicine ──────────────────────────────────────────────────────
        private void AddMedicine()
        {
            if (string.IsNullOrWhiteSpace(_newMedicineLabel)) return;

            var label = _newMedicineLabel.Trim();
            var time  = TimeOnly.TryParse(label, out var t) ? t : TimeOnly.MinValue;

            _draft.MedicineSchedule.Add(new MedicineScheduleItemDto
            {
                Label   = label,
                Time    = time,
                IsGiven = false
            });

            // Keep sorted 00:00 → 23:59
            _draft.MedicineSchedule.Sort((a, b) => a.Time.CompareTo(b.Time));

            _newMedicineLabel    = string.Empty;
            _showNewMedicineForm = false;
        }

        // ── PN ────────────────────────────────────────────────────────────
        private void AddPN()
        {
            _pnRows.Add(new PnRow
            {
                Entry     = new PNEntryDto { IssuedBy = "Logged-in user" },
                IsEditing = true,
                Snapshot  = null
            });
        }

        private void SavePnRow(PnRow row)
        {
            row.Snapshot  = ClonePN(row.Entry);
            row.IsEditing = false;
            _pnRows.Sort((a, b) => ParseTime(a.Entry.TimeOfAdministration).CompareTo(ParseTime(b.Entry.TimeOfAdministration)));
        }

        private void CancelPnRow(PnRow row)
        {
            if (row.Snapshot is null)
            {
                _pnRows.Remove(row);
            }
            else
            {
                row.Entry.TimeOfAdministration = row.Snapshot.TimeOfAdministration;
                row.Entry.Medication           = row.Snapshot.Medication;
                row.Entry.Reason               = row.Snapshot.Reason;
                row.IsEditing                  = false;
            }
        }

        // ── Staff ─────────────────────────────────────────────────────────
        private void AddStaff(string staffMember)
        {
            if (!_draft.AssignedStaff.Contains(staffMember))
                _draft.AssignedStaff.Add(staffMember);
            _showStaffPicker = false;
        }

        private void RemoveStaff(string staffMember)
        {
            _draft.AssignedStaff.Remove(staffMember);
        }

        // ── Save ─────────────────────────────────────────────────────────
        private async Task SaveChanges()
        {
            Resident.RiskLevel        = _draft.RiskLevel;
            Resident.LatestStatusNote = _draft.LatestStatusNote;
            Resident.GroceryDay       = _draft.GroceryDay;
            Resident.AssignedStaff    = new List<string>(_draft.AssignedStaff);
            Resident.MedicineSchedule = _draft.MedicineSchedule
                .Select(m => new MedicineScheduleItemDto { Label = m.Label, Time = m.Time, IsGiven = m.IsGiven })
                .ToList();
            Resident.PNEntries = _pnRows
                .OrderBy(r => ParseTime(r.Entry.TimeOfAdministration))
                .Select(r => r.Entry)
                .ToList();

            await OnSaved.InvokeAsync();
        }

        // ── Time helpers ──────────────────────────────────────────────────
        private static TimeOnly ParseTime(string? s) =>
            TimeOnly.TryParse(s, out var t) ? t : TimeOnly.MaxValue;

        // ── Time auto-format ──────────────────────────────────────────────
        /// <summary>
        /// Formats a raw time string so that `:` is always the 3rd-to-last character.
        /// "845" → "8:45"   |   "0845" → "8:45"   |   "08:45" → "8:45"
        /// </summary>
        private static string FormatTime(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            var digits = new string(raw.Where(char.IsDigit).ToArray());

            if (digits.Length < 3) return raw.Trim();

            var minutes  = digits[^2..];
            var hoursStr = digits[..^2];

            if (!int.TryParse(hoursStr, out var hours) ||
                !int.TryParse(minutes,  out var mins))
                return raw.Trim();

            if (hours > 23 || mins > 59) return raw.Trim();

            return $"{hours}:{minutes}";
        }

        // ── Deep copy helpers ─────────────────────────────────────────────
        private static ResidentCardDto DeepCopy(ResidentCardDto src) => new()
        {
            ResidentStatusID = src.ResidentStatusID,
            ResidentID       = src.ResidentID,
            Date             = src.Date,
            ResidentName     = src.ResidentName,
            IsActive         = src.IsActive,
            RiskLevel        = src.RiskLevel,
            LatestStatusNote = src.LatestStatusNote,
            PaymentMethod    = src.PaymentMethod,
            GroceryDay       = src.GroceryDay,
            AssignedStaff    = new List<string>(src.AssignedStaff),
            MedicineSchedule = src.MedicineSchedule
                .Select(m => new MedicineScheduleItemDto { Label = m.Label, Time = m.Time, IsGiven = m.IsGiven })
                .ToList(),
            PNEntries = src.PNEntries.Select(ClonePN).ToList(),
        };

        private static PNEntryDto ClonePN(PNEntryDto src) => new()
        {
            TimeOfAdministration = src.TimeOfAdministration,
            Medication           = src.Medication,
            Reason               = src.Reason,
            IssuedBy             = src.IssuedBy,
        };
    }
}
