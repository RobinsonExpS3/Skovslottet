using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
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
            var latestId = await _context.ShiftBoards
                .AsNoTracking()
                .OrderByDescending(sb => sb.StartDateTime)
                .Select(sb => sb.ShiftBoardID)
                .FirstOrDefaultAsync(ct);

            return latestId == Guid.Empty
                ? null
                : await GetByIdAsync(latestId, ct);
        }

        public async Task<ShiftBoardDTO?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var shiftBoard = await GetShiftBoardAsync(id, ct);
            if (shiftBoard is null)
            {
                return null;
            }

            var departmentId = shiftBoard.StaffShifts
                .Select(ss => ss.Staff.DepartmentID)
                .FirstOrDefault();

            var department = departmentId == Guid.Empty
                ? null
                : await GetDepartmentAsync(departmentId, ct);

            var residents = await GetActiveResidentsAsync(ct);

            return new ShiftBoardDTO
            {
                ShiftBoardId = shiftBoard.ShiftBoardID,
                ShiftType = shiftBoard.ShiftType,
                StartDate = shiftBoard.StartDateTime,
                EndDate = shiftBoard.EndDateTime,
                DepartmentName = department?.DepartmentName ?? string.Empty,
                PhoneNumbers = department?.Phones
                    .Select(phone => new PhoneEntryDto
                    {
                        Number = phone.PhoneNumber,
                        StaffName = phone.StaffPhones
                            .OrderByDescending(sp => sp.AssignedAt)
                            .Select(sp => sp.Staff.StaffName)
                            .FirstOrDefault() ?? string.Empty
                    })
                    .ToList() ?? new(),
                DepartmentTasks = department?.DepartmentTasks
                    .Select(task => task.DepartmentTaskName)
                    .ToList() ?? new(),
                AllStaff = department?.Staffs
                    .Select(staff => staff.StaffName)
                    .OrderBy(name => name)
                    .ToList() ?? new(),

                ResidentCards = residents
                    .Select(MapResidentCard)
                    .ToList()
            };
        }

        public async Task<IEnumerable<ShiftBoard>> GetAllShiftBoardsAsync(CancellationToken ct = default)
        {
            return await _context.ShiftBoards
                .AsNoTracking()
                .OrderBy(s => s.StartDateTime)
                .ToListAsync(ct);
        }

        public async Task<ShiftBoard?> GetShiftBoardByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.ShiftBoards
                .AsNoTracking()
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);
        }

        public async Task<ShiftBoard> CreateShiftBoardAsync(ShiftBoard shiftBoard, CancellationToken ct = default)
        {
            _context.ShiftBoards.Add(shiftBoard);
            await _context.SaveChangesAsync(ct);
            return shiftBoard;
        }

        public async Task<bool> UpdateShiftBoardAsync(Guid id, ShiftBoard shiftBoard, CancellationToken ct = default)
        {
            var existingShiftBoard = await _context.ShiftBoards
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);

            if (existingShiftBoard == null)
            {
                return false;
            }

            existingShiftBoard.ShiftType = shiftBoard.ShiftType;
            existingShiftBoard.StartDateTime = shiftBoard.StartDateTime;
            existingShiftBoard.EndDateTime = shiftBoard.EndDateTime;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteShiftBoardAsync(Guid id, CancellationToken ct = default)
        {
            var shiftBoard = await _context.ShiftBoards
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);

            if (shiftBoard == null)
            {
                return false;
            }

            _context.ShiftBoards.Remove(shiftBoard);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private async Task<ShiftBoard?> GetShiftBoardAsync(Guid id, CancellationToken ct)
        {
            return await _context.ShiftBoards
                .AsNoTracking()
                .Include(sb => sb.StaffShifts)
                    .ThenInclude(ss => ss.Staff)
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);
        }

        private async Task<Department?> GetDepartmentAsync(Guid id, CancellationToken ct)
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.DepartmentTasks)
                .Include(d => d.Staffs)
                .Include(d => d.Phones)
                    .ThenInclude(p => p.StaffPhones)
                        .ThenInclude(sp => sp.Staff)
                .FirstOrDefaultAsync(d => d.DepartmentID == id, ct);
        }

        private async Task<List<Resident>> GetActiveResidentsAsync(CancellationToken ct)
        {
            return await _context.Residents
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
        }

        private static ResidentCardDto MapResidentCard(Resident resident)
        {
            var today = DateTime.Today;
            var latestStatus = resident.ResidentStatuses
                .OrderByDescending(status => status.Date)
                .FirstOrDefault();

            return new ResidentCardDto
            {
                ResidentStatusID = latestStatus?.ResidentStatusID ?? Guid.Empty,
                ResidentID = resident.ResidentID,
                Date = today,
                ResidentName = resident.ResidentName,
                IsActive = resident.IsActive,
                RiskLevel = latestStatus?.RiskLevel?.RiskLevelName,
                LatestStatusNote = latestStatus?.Status,
                GroceryDay = resident.GroceryDay?.GroceryDayName,
                PaymentMethod = resident.ResidentPaymentMethods
                    .Select(rpm => rpm.PaymentMethod?.PaymentMethodName)
                    .FirstOrDefault(),
                AssignedStaff = latestStatus?.StaffResidentStatuses
                    .Select(srs => srs.Staff.StaffName)
                    .OrderBy(name => name)
                    .ToList() ?? new(),
                MedicineSchedule = MapMedicineSchedule(resident, today),
                PNEntries = MapPNEntries(resident, today)
            };
        }

        private static List<MedicineScheduleItemDto> MapMedicineSchedule(Resident resident, DateTime date)
        {
            return resident.Medicines
                .Where(medicine => medicine.MedicineTime.Date == date)
                .OrderBy(medicine => medicine.MedicineTime)
                .Select(medicine => new MedicineScheduleItemDto
                {
                    Label = medicine.MedicineTime.ToString("HH:mm"),
                    Time = TimeOnly.FromDateTime(medicine.MedicineTime),
                    IsGiven = medicine.MedicineGivenTime != null
                })
                .ToList();
        }

        private static List<PNEntryDto> MapPNEntries(Resident resident, DateTime date)
        {
            return resident.PNs
                .Where(pn => pn.PNGivenTime.Date == date)
                .OrderBy(pn => pn.PNGivenTime)
                .Select(pn => new PNEntryDto
                {
                    TimeOfAdministration = pn.PNGivenTime.ToString("HH:mm"),
                    Medication = string.Empty,
                    Reason = pn.PNReason,
                    IssuedBy = pn.StaffPNs
                        .Select(spn => spn.Staff.StaffName)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToList();
        }
    }
}
