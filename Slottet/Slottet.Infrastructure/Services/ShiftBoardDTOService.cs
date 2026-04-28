using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class ShiftBoardDTOService : IShiftBoardDTOService
    {
        private readonly SlottetDBContext _context;

        public ShiftBoardDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<ShiftBoardDTO?> GetLatestAsync(CancellationToken ct = default)
        {
            var latest = await _context.ShiftBoards
                .AsNoTracking()
                .OrderByDescending(sb => sb.StartDateTime)
                .Select(sb => sb.ShiftBoardID)
                .FirstOrDefaultAsync(ct);

            if (latest == Guid.Empty) return null;

            return await GetByIdAsync(latest, ct);
        }

        public async Task<ShiftBoardDTO?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            // ── 1. Load ShiftBoard + SpecialResponsibilities + department ID ──
            var shiftBoard = await _context.ShiftBoards
                .AsNoTracking()
                .Include(sb => sb.StaffShifts)
                    .ThenInclude(ss => ss.Staff)
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);

            if (shiftBoard is null) return null;

            var departmentId = shiftBoard.StaffShifts
                .Select(ss => ss.Staff.DepartmentID)
                .FirstOrDefault();

            // ── 2. Load department data separately to avoid include cycles ──
            var department = departmentId != Guid.Empty
                ? await _context.Departments
                    .AsNoTracking()
                    .Where(d => d.DepartmentID == departmentId)
                    .Include(d => d.DepartmentTasks)
                    .Include(d => d.Staffs)
                    .Include(d => d.Phones)
                        .ThenInclude(p => p.StaffPhones)
                            .ThenInclude(sp => sp.Staff)
                    .FirstOrDefaultAsync(ct)
                : null;

            // ── 3. Load all active residents with related data ────────────
            var today = DateTime.Today;

            var residents = await _context.Residents
                .AsNoTracking()
                .Where(r => r.IsActive)
                .Include(r => r.GroceryDay)
                .Include(r => r.Medicines)
                .Include(r => r.PNs)
                    .ThenInclude(pn => pn.StaffPNs)
                        .ThenInclude(spn => spn.Staff)
                .Include(r => r.ResidentPaymentMethods)
                    .ThenInclude(rpm => rpm.PaymentMethod)
                .Include(r => r.ResidentStatuses)
                    .ThenInclude(rs => rs.RiskLevel)
                .Include(r => r.ResidentStatuses)
                    .ThenInclude(rs => rs.StaffResidentStatuses)
                        .ThenInclude(srs => srs.Staff)
                .ToListAsync(ct);

            // ── 4. Assemble DTO ───────────────────────────────────────────
            return new ShiftBoardDTO
            {
                ShiftBoardId   = shiftBoard.ShiftBoardID,
                ShiftType      = shiftBoard.ShiftType,
                StartDate      = shiftBoard.StartDateTime,
                EndDate        = shiftBoard.EndDateTime,
                DepartmentName = department?.DepartmentName ?? string.Empty,

                PhoneNumbers = department?.Phones
                    .Select(p => new PhoneEntryDto
                    {
                        Number    = p.PhoneNumber,
                        StaffName = p.StaffPhones
                            .OrderByDescending(sp => sp.AssignedAt)
                            .Select(sp => sp.Staff.StaffName)
                            .FirstOrDefault() ?? string.Empty
                    })
                    .ToList() ?? new(),

                DepartmentTasks = department?.DepartmentTasks
                    .Select(dt => dt.DepartmentTaskName)
                    .ToList() ?? new(),

                AllStaff = department?.Staffs
                    .Select(s => s.StaffName)
                    .OrderBy(n => n)
                    .ToList() ?? new(),

                ResidentCards = residents.Select(r =>
                {
                    var latestStatus = r.ResidentStatuses
                        .OrderByDescending(rs => rs.Date)
                        .FirstOrDefault();

                    var todayMeds = r.Medicines
                        .Where(m => m.MedicineTime.Date == today)
                        .OrderBy(m => m.MedicineTime)
                        .ToList();

                    var todayPns = r.PNs
                        .Where(p => p.PNGivenTime.Date == today)
                        .OrderBy(p => p.PNGivenTime)
                        .ToList();

                    return new ResidentCardDto
                    {
                        ResidentCardId   = latestStatus?.ResidentStatusID ?? r.ResidentID,
                        ResidentId       = r.ResidentID,
                        Date             = today,
                        ResidentName     = r.ResidentName,
                        IsActive         = r.IsActive,
                        RiskLevel        = latestStatus?.RiskLevel?.RiskLevelName,
                        LatestStatusNote = latestStatus?.Status,
                        GroceryDay       = r.GroceryDay?.GroceryDayName,

                        PaymentMethod = r.ResidentPaymentMethods
                            .Select(rpm => rpm.PaymentMethod?.PaymentMethodName)
                            .FirstOrDefault(),

                        AssignedStaff = latestStatus?.StaffResidentStatuses
                            .Select(srs => srs.Staff.StaffName)
                            .ToList() ?? new(),

                        MedicineSchedule = todayMeds
                            .Select(m => new ResidentCardDto.MedicineScheduleItemDto
                            {
                                Label   = m.MedicineTime.ToString("HH:mm"),
                                Time    = TimeOnly.FromDateTime(m.MedicineTime),
                                IsGiven = m.MedicineGivenTime != default
                            })
                            .ToList(),

                        PNEntries = todayPns
                            .Select(pn => new ResidentCardDto.PNEntryDto
                            {
                                TimeOfAdministration = pn.PNGivenTime.ToString("HH:mm"),
                                Medication           = string.Empty,
                                Reason               = pn.PNReason,
                                IssuedBy             = pn.StaffPNs
                                    .Select(spn => spn.Staff.StaffName)
                                    .FirstOrDefault() ?? string.Empty
                            })
                            .ToList()
                    };
                }).ToList()
            };
        }
    }
}
