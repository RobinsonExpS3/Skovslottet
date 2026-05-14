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

        /// <summary>
        /// Sends a query to the database to retrieve the latest shift board and maps it to a DTO object.
        /// </summary>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns the latest ShiftBoardDTO object if found, otherwise null.</returns>
        public async Task<ShiftBoardDTO?> GetShiftBoardByDateAndShiftAsync(DateOnly date, string shiftType, CancellationToken ct = default)
        {
            var start = date.ToDateTime(TimeOnly.MinValue);
            var end   = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var id = await _context.ShiftBoards
                .AsNoTracking()
                .Where(sb => sb.ShiftType == shiftType
                          && sb.StartDateTime >= start
                          && sb.StartDateTime < end)
                .Select(sb => sb.ShiftBoardID)
                .FirstOrDefaultAsync(ct);

            return id == Guid.Empty ? null : await GetShiftBoardDtoByIdAsync(id, ct);
        }

        public async Task<ShiftBoardDTO?> GetCurrentShiftBoardAsync(CancellationToken ct = default)
        {
            var latestId = await _context.ShiftBoards
                .AsNoTracking()
                .OrderByDescending(sb => sb.StartDateTime)
                .Select(sb => sb.ShiftBoardID)
                .FirstOrDefaultAsync(ct);

            return latestId == Guid.Empty
                ? null
                : await GetShiftBoardDtoByIdAsync(latestId, ct);
        }

        /// <summary>
        /// Sends a query to the database to retrieve a shift board based on a specific ID and maps it to a DTO object.
        /// </summary>
        /// <param name="id">The ID of the shift board to retrieve.</param>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns a ShiftBoardDTO object if found, otherwise null.</returns>
        public async Task<ShiftBoardDTO?> GetShiftBoardDtoByIdAsync(Guid id, CancellationToken ct = default)
        {
            var shiftBoard = await GetShiftBoardAsync(id, ct);
            if (shiftBoard is null)
            {
                return null;
            }

            var department = shiftBoard.DepartmentID.HasValue
                ? await GetDepartmentAsync(shiftBoard.DepartmentID.Value, ct)
                : null;

            var shiftDate = DateOnly.FromDateTime(shiftBoard.StartDateTime);
            var residents = await GetActiveResidentsAsync(shiftDate, ct);

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
                SpecialResponsibilities = department is not null
                    ? await GetSpecialResponsibilitiesForDepartmentAsync(department.DepartmentID, ct)
                    : new(),
                AllStaff = department?.Staffs
                    .Select(staff => staff.StaffName)
                    .OrderBy(name => name)
                    .ToList() ?? new(),

                ResidentCards = residents
                    .Select(r => MapResidentCard(r, shiftDate))
                    .ToList()
            };
        }

        /// <summary>
        /// Sends a query to the database to retrieve all shift board objects.
        /// </summary>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns a list of ShiftBoard objects.</returns>
        public async Task<IEnumerable<ShiftBoardEntryDTO>> GetAllShiftBoardsAsync(CancellationToken ct = default)
        {
            return await _context.ShiftBoards
                .AsNoTracking()
                .OrderBy(s => s.StartDateTime)
                .Select(shiftBoard => new ShiftBoardEntryDTO
                {
                    ShiftBoardID = shiftBoard.ShiftBoardID,
                    ShiftType = shiftBoard.ShiftType,
                    StartDateTime = shiftBoard.StartDateTime,
                    EndDateTime = shiftBoard.EndDateTime
                })
                .ToListAsync(ct);
        }

        /// <summary>
        /// Sends a query to the database to retrieve a shift board object based on a specific ID.
        /// </summary>
        /// <param name="id">The ID of the shift board to retrieve.</param>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns a ShiftBoard object if found, otherwise null.</returns>
        public async Task<ShiftBoardEntryDTO?> GetShiftBoardByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.ShiftBoards
                .AsNoTracking()
                .Where(sb => sb.ShiftBoardID == id)
                .Select(shiftBoard => new ShiftBoardEntryDTO
                {
                    ShiftBoardID = shiftBoard.ShiftBoardID,
                    ShiftType = shiftBoard.ShiftType,
                    StartDateTime = shiftBoard.StartDateTime,
                    EndDateTime = shiftBoard.EndDateTime
                })
                .FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Creates a new shift board object in the database.
        /// </summary>
        /// <param name="dto">DTO object containing shift board information.</param>
        /// <param name="ct">Cancellation token used to cancel the database operation.</param>
        /// <returns>Returns the created ShiftBoardEntryDTO object.</returns>
        public async Task<ShiftBoardEntryDTO?> PostShiftBoardAsync(ShiftBoardEntryDTO dto, CancellationToken ct = default)
        {
            var shiftBoard = new ShiftBoard
            {
                ShiftBoardID  = dto.ShiftBoardID == Guid.Empty ? Guid.NewGuid() : dto.ShiftBoardID,
                ShiftType     = dto.ShiftType,
                StartDateTime = dto.StartDateTime,
                EndDateTime   = dto.EndDateTime,
                DepartmentID  = dto.DepartmentID
            };

            _context.ShiftBoards.Add(shiftBoard);
            await _context.SaveChangesAsync(ct);

            var createdShiftBoard = await GetShiftBoardByIdAsync(shiftBoard.ShiftBoardID, ct);
            return createdShiftBoard;
        }

        /// <summary>
        /// Updates a shift board object in the database based on the provided ID and ShiftBoard object.
        /// </summary>
        /// <param name="id">The ID of the shift board to update.</param>
        /// <param name="dto">DTO object containing updated shift board information.</param>
        /// <param name="ct">Cancellation token used to cancel the database operation.</param>
        /// <returns>Returns true if the update is successful, otherwise false.</returns>
        public async Task<bool> PutShiftBoardAsync(Guid id, ShiftBoardEntryDTO dto, CancellationToken ct = default)
        {
            var existingShiftBoard = await _context.ShiftBoards
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);

            if (existingShiftBoard == null)
            {
                return false;
            }

            existingShiftBoard.ShiftType     = dto.ShiftType;
            existingShiftBoard.StartDateTime = dto.StartDateTime;
            existingShiftBoard.EndDateTime   = dto.EndDateTime;
            existingShiftBoard.DepartmentID  = dto.DepartmentID;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Deletes a shift board object from the database based on the given ID.
        /// </summary>
        /// <param name="id">The ID of the shift board to delete.</param>
        /// <param name="ct">Cancellation token used to cancel the database operation.</param>
        /// <returns>Returns true if the deletion is successful, otherwise false.</returns>
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

        /// <summary>
        /// Gets a shift board with the related staff shift and staff data needed for DTO mapping.
        /// </summary>
        /// <param name="id">The ID of the shift board to retrieve.</param>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns a ShiftBoard object if found, otherwise null.</returns>
        private async Task<ShiftBoard?> GetShiftBoardAsync(Guid id, CancellationToken ct)
        {
            return await _context.ShiftBoards
                .AsNoTracking()
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);
        }

        /// <summary>
        /// Gets a department with the related task, staff, and phone data needed for DTO mapping.
        /// </summary>
        /// <param name="id">The ID of the department to retrieve.</param>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns a Department object if found, otherwise null.</returns>
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

        /// <summary>
        /// Gets all special responsibilities assigned to a department.
        /// </summary>
        private async Task<List<SpecialResponsibilityEntryDto>> GetSpecialResponsibilitiesForDepartmentAsync(Guid departmentId, CancellationToken ct)
        {
            return await _context.SpecialResponsibilities
                .AsNoTracking()
                .Where(sr => sr.SpecialResponsibilityStaffs.Any(srs => srs.DepartmentID == departmentId))
                .OrderBy(sr => sr.TaskName)
                .Select(sr => new SpecialResponsibilityEntryDto
                {
                    SpecialResponsibilityID = sr.SpecialResponsibilityID,
                    Description = sr.TaskName,
                    StaffName = sr.SpecialResponsibilityStaffs
                        .Where(srs => srs.DepartmentID == departmentId)
                        .OrderByDescending(srs => srs.AssignedAt)
                        .Select(srs => srs.Staff.StaffName)
                        .FirstOrDefault() ?? string.Empty,
                    StaffInitials = sr.SpecialResponsibilityStaffs
                        .Where(srs => srs.DepartmentID == departmentId)
                        .OrderByDescending(srs => srs.AssignedAt)
                        .Select(srs => srs.Staff.Initials)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToListAsync(ct);
        }

        /// <summary>
        /// Gets all active residents with the related data needed for resident card mapping.
        /// </summary>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns a list of active Resident objects.</returns>
        private async Task<List<Resident>> GetActiveResidentsAsync(DateOnly date, CancellationToken ct)
        {
            var dateAsDateTime = date.ToDateTime(TimeOnly.MinValue);

            return await _context.Residents
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.ResidentID)
                .Include(r => r.GroceryDay)
                .Include(r => r.Medicines)
                    .ThenInclude(m => m.MedicineLogs.Where(ml => ml.Date == date))
                .Include(r => r.PNs.Where(pn => pn.PNGivenTime.Date == dateAsDateTime.Date))
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

        /// <summary>
        /// Creates a new ResidentCardDto instance by mapping the properties of the specified Resident entity.
        /// </summary>
        /// <param name="resident">The Resident entity to map from. Cannot be null.</param>
        /// <returns>A ResidentCardDto containing the mapped values from the specified Resident entity.</returns>
        private static ResidentCardDto MapResidentCard(Resident resident, DateOnly shiftDate)
        {
            var shiftDateTime = shiftDate.ToDateTime(TimeOnly.MinValue);

            // Find den status der var aktiv på vagtens dato — dvs. nyeste status oprettet på eller før datoen.
            var statusAtShift = resident.ResidentStatuses
                .Where(s => DateOnly.FromDateTime(s.Date) <= shiftDate)
                .OrderByDescending(s => s.Date)
                .FirstOrDefault();

            return new ResidentCardDto
            {
                ResidentStatusID = statusAtShift?.ResidentStatusID ?? Guid.Empty,
                ResidentID = resident.ResidentID,
                Date = shiftDateTime,
                ResidentName = resident.ResidentName,
                IsActive = resident.IsActive,
                RiskLevel = statusAtShift?.RiskLevel?.RiskLevelName,
                LatestStatusNote = statusAtShift?.Status,
                GroceryDay = resident.GroceryDay?.GroceryDayName,
                PaymentMethod = resident.ResidentPaymentMethods
                    .Select(rpm => rpm.PaymentMethod?.PaymentMethodName)
                    .FirstOrDefault(),
                AssignedStaff = statusAtShift?.StaffResidentStatuses
                    .Select(srs => srs.Staff.StaffName)
                    .OrderBy(name => name)
                    .ToList() ?? new(),
                MedicineSchedule = MapMedicineSchedule(resident, shiftDateTime),
                PNEntries = MapPNEntries(resident, shiftDateTime)
            };
        }

        /// <summary>
        /// Creates medicine schedule DTO objects for a resident on the specified date.
        /// </summary>
        /// <param name="resident">The Resident entity containing medicine information.</param>
        /// <param name="date">The date used to filter medicine times.</param>
        /// <returns>Returns a list of MedicineScheduleItemDto objects.</returns>
        private static List<MedicineScheduleItemDto> MapMedicineSchedule(Resident resident, DateTime date)
        {
            var today = DateOnly.FromDateTime(date);

            return resident.Medicines
                .OrderBy(m => m.ScheduledTime)
                .Select(m =>
                {
                    var log = m.MedicineLogs.FirstOrDefault(ml => ml.Date == today);
                    return new MedicineScheduleItemDto
                    {
                        Label   = m.ScheduledTime.ToString("HH:mm"),
                        Time    = m.ScheduledTime,
                        IsGiven = log?.GivenTime != null
                    };
                })
                .ToList();
        }

        /// <summary>
        /// Creates PN entry DTO objects for a resident on the specified date.
        /// </summary>
        /// <param name="resident">The Resident entity containing PN information.</param>
        /// <param name="date">The date used to filter PN entries.</param>
        /// <returns>Returns a list of PNEntryDto objects.</returns>
        private static List<PNEntryDto> MapPNEntries(Resident resident, DateTime date)
        {
            return resident.PNs
                .Where(pn => pn.PNGivenTime.Date == date.Date)
                .OrderBy(pn => pn.PNGivenTime)
                .Select(pn => new PNEntryDto
                {
                    TimeOfAdministration = pn.PNGivenTime.ToString("HH:mm"),
                    Medication           = pn.PNMedication,
                    Reason               = pn.PNReason,
                    IssuedBy             = pn.StaffPNs
                                              .Select(spn => spn.Staff.StaffName)
                                              .FirstOrDefault() ?? string.Empty
                })
                .ToList();
        }
    }
}
