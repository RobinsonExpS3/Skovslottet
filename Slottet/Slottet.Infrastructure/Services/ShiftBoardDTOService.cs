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

        public async Task<ShiftBoardDTO?> GetPreviousShiftBoardAsync(Guid currentId, CancellationToken ct = default)
        {
            var current = await _context.ShiftBoards
                .AsNoTracking()
                .Where(sb => sb.ShiftBoardID == currentId)
                .Select(sb => sb.StartDateTime)
                .FirstOrDefaultAsync(ct);

            if (current == default) return null;

            var prevId = await _context.ShiftBoards
                .AsNoTracking()
                .Where(sb => sb.StartDateTime < current)
                .OrderByDescending(sb => sb.StartDateTime)
                .Select(sb => sb.ShiftBoardID)
                .FirstOrDefaultAsync(ct);

            return prevId == Guid.Empty ? null : await GetShiftBoardDtoByIdAsync(prevId, ct);
        }

        public async Task<ShiftBoardDTO?> GetNextShiftBoardAsync(Guid currentId, CancellationToken ct = default)
        {
            var current = await _context.ShiftBoards
                .AsNoTracking()
                .Where(sb => sb.ShiftBoardID == currentId)
                .Select(sb => sb.StartDateTime)
                .FirstOrDefaultAsync(ct);

            if (current == default) return null;

            var nextId = await _context.ShiftBoards
                .AsNoTracking()
                .Where(sb => sb.StartDateTime > current)
                .OrderBy(sb => sb.StartDateTime)
                .Select(sb => sb.ShiftBoardID)
                .FirstOrDefaultAsync(ct);

            return nextId == Guid.Empty ? null : await GetShiftBoardDtoByIdAsync(nextId, ct);
        }

        public async Task<ShiftBoardDTO?> GetOrCreateNextShiftBoardAsync(Guid currentId, CancellationToken ct = default)
        {
            var current = await _context.ShiftBoards
                .AsNoTracking()
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == currentId, ct);

            if (current is null) return null;

            // Find existing next board
            var nextId = await _context.ShiftBoards
                .AsNoTracking()
                .Where(sb => sb.StartDateTime > current.StartDateTime)
                .OrderBy(sb => sb.StartDateTime)
                .Select(sb => sb.ShiftBoardID)
                .FirstOrDefaultAsync(ct);

            if (nextId != Guid.Empty)
                return await GetShiftBoardDtoByIdAsync(nextId, ct);

            // None exists — create the next shift slot from current's EndDateTime
            var nextStart = current.EndDateTime;
            var nextType  = nextStart.Hour switch
            {
                >= 7 and < 15  => "Dag",
                >= 15 and < 23 => "Aften",
                _              => "Nat"
            };
            var nextEnd = nextType == "Nat"
                ? nextStart.Date.AddDays(1).AddHours(7)
                : nextStart.AddHours(8);

            var newBoard = new ShiftBoard
            {
                ShiftBoardID  = Guid.NewGuid(),
                ShiftType     = nextType,
                StartDateTime = nextStart,
                EndDateTime   = nextEnd,
                DepartmentID  = current.DepartmentID
            };

            _context.ShiftBoards.Add(newBoard);
            await _context.SaveChangesAsync(ct);

            return await GetShiftBoardDtoByIdAsync(newBoard.ShiftBoardID, ct);
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

            var shiftDate = WorkDayDate(shiftBoard.StartDateTime);
            var residents = await GetActiveResidentsAsync(shiftDate, shiftBoard.ShiftBoardID, ct);

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
                        PhoneID = phone.PhoneID,
                        Number = phone.PhoneNumber,
                        StaffName = phone.StaffPhones
                            .Where(sp => sp.ShiftBoardID == shiftBoard.ShiftBoardID)
                            .Select(sp => sp.Staff.StaffName)
                            .FirstOrDefault() ?? string.Empty
                    })
                    .ToList() ?? new(),
                DepartmentTasks = department?.DepartmentTasks
                    .Select(task => task.DepartmentTaskName)
                    .ToList() ?? new(),
                SpecialResponsibilities = department is not null
                    ? await GetSpecialResponsibilitiesForShiftBoardAsync(department.DepartmentID, shiftBoard.ShiftBoardID, ct)
                    : new(),
                AllStaff = await _context.Staffs
                    .AsNoTracking()
                    .OrderBy(s => s.StaffName)
                    .Select(s => s.StaffName)
                    .ToListAsync(ct),

                ResidentCards = residents
                    .Select(r => MapResidentCard(r, shiftBoard.StartDateTime, shiftBoard.ShiftBoardID))
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
        private async Task<List<SpecialResponsibilityEntryDto>> GetSpecialResponsibilitiesForShiftBoardAsync(Guid departmentId, Guid shiftBoardId, CancellationToken ct)
        {
            return await _context.SpecialResponsibilities
                .AsNoTracking()
                .OrderBy(sr => sr.TaskName)
                .Select(sr => new SpecialResponsibilityEntryDto
                {
                    SpecialResponsibilityID = sr.SpecialResponsibilityID,
                    DepartmentID = sr.DepartmentID,
                    Description = sr.TaskName,
                    StaffName = sr.SpecialResponsibilityStaffs
                        .Where(srs => srs.ShiftBoardID == shiftBoardId)
                        .Select(srs => srs.Staff.StaffName)
                        .FirstOrDefault() ?? string.Empty,
                    StaffInitials = sr.SpecialResponsibilityStaffs
                        .Where(srs => srs.ShiftBoardID == shiftBoardId)
                        .Select(srs => srs.Staff.Initials)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToListAsync(ct);
        }

        public async Task<bool> UpdatePhoneAssignmentAsync(SwapPhoneDTO dto, CancellationToken ct = default)
        {
            if (dto.PhoneID == Guid.Empty || dto.ShiftBoardID == Guid.Empty)
                return false;

            var existing = await _context.StaffPhones
                .Where(sp => sp.PhoneID == dto.PhoneID && sp.ShiftBoardID == dto.ShiftBoardID)
                .ToListAsync(ct);

            _context.StaffPhones.RemoveRange(existing);

            if (!string.IsNullOrWhiteSpace(dto.StaffName))
            {
                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.StaffName == dto.StaffName, ct);

                if (staff is null) return false;

                _context.StaffPhones.Add(new StaffPhone
                {
                    PhoneID      = dto.PhoneID,
                    StaffID      = staff.StaffID,
                    ShiftBoardID = dto.ShiftBoardID,
                    AssignedAt   = DateTime.Now
                });
            }

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> UpdateSpecialResponsibilityAssignmentAsync(SpecialResponsibilityAssignmentDto dto, CancellationToken ct = default)
        {
            if (dto.SpecialResponsibilityID == Guid.Empty || dto.ShiftBoardID == Guid.Empty)
                return false;

            var existing = await _context.SpecialResponsibilityStaffs
                .Where(srs => srs.SpecialResponsibilityID == dto.SpecialResponsibilityID
                           && srs.ShiftBoardID == dto.ShiftBoardID)
                .ToListAsync(ct);

            _context.SpecialResponsibilityStaffs.RemoveRange(existing);

            if (!string.IsNullOrWhiteSpace(dto.StaffName))
            {
                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.StaffName == dto.StaffName, ct);

                if (staff is null) return false;

                _context.SpecialResponsibilityStaffs.Add(new SpecialResponsibilityStaff
                {
                    SpecialResponsibilityID = dto.SpecialResponsibilityID,
                    StaffID                 = staff.StaffID,
                    ShiftBoardID            = dto.ShiftBoardID,
                    DepartmentID            = dto.DepartmentID,
                    AssignedAt              = DateTime.Now
                });
            }

            await _context.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Gets all active residents with the related data needed for resident card mapping.
        /// </summary>
        /// <param name="ct">Cancellation token used to cancel the database query.</param>
        /// <returns>Returns a list of active Resident objects.</returns>
        private async Task<List<Resident>> GetActiveResidentsAsync(DateOnly date, Guid shiftBoardId, CancellationToken ct)
        {
            return await _context.Residents
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.ResidentID)
                .Include(r => r.GroceryDay)
                .Include(r => r.Medicines)
                    .ThenInclude(m => m.MedicineLogs.Where(ml => ml.Date == date))
                .Include(r => r.PNs)
                    .ThenInclude(pn => pn.StaffPNs)
                        .ThenInclude(spn => spn.Staff)
                .Include(r => r.ResidentPaymentMethods)
                    .ThenInclude(rpm => rpm.PaymentMethod)
                .Include(r => r.ResidentStatuses)
                    .ThenInclude(rs => rs.RiskLevel)
                .Include(r => r.ResidentStatuses)
                    .ThenInclude(rs => rs.StaffResidentStatuses.Where(srs => srs.ShiftBoardID == shiftBoardId))
                        .ThenInclude(srs => srs.Staff)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Creates a new ResidentCardDto instance by mapping the properties of the specified Resident entity.
        /// </summary>
        /// <param name="resident">The Resident entity to map from. Cannot be null.</param>
        /// <returns>A ResidentCardDto containing the mapped values from the specified Resident entity.</returns>
        private static ResidentCardDto MapResidentCard(Resident resident, DateTime startDateTime, Guid shiftBoardId)
        {
            var shiftDate = WorkDayDate(startDateTime);

            // Find den status der var aktiv på vagtens dato — dvs. nyeste status oprettet på eller før datoen.
            var statusAtShift = resident.ResidentStatuses
                .Where(s => DateOnly.FromDateTime(s.Date) <= shiftDate)
                .OrderByDescending(s => s.Date)
                .FirstOrDefault();

            return new ResidentCardDto
            {
                ResidentStatusID = statusAtShift?.ResidentStatusID ?? Guid.Empty,
                ResidentID = resident.ResidentID,
                ShiftBoardID = shiftBoardId,
                Date = startDateTime,
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
                MedicineSchedule = MapMedicineSchedule(resident, startDateTime),
                PNEntries = MapPNEntries(resident, startDateTime)
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
            var today = WorkDayDate(date);

            return resident.Medicines
                .OrderBy(m => m.ScheduledTime.Hour < 7 ? m.ScheduledTime.Hour + 24 : m.ScheduledTime.Hour)
                .ThenBy(m => m.ScheduledTime.Minute)
                .Select(m =>
                {
                    var log = m.MedicineLogs.FirstOrDefault(ml => ml.Date == today);
                    return new MedicineScheduleItemDto
                    {
                        MedicineID = m.MedicineID,
                        Label      = m.ScheduledTime.ToString("HH:mm"),
                        Time       = m.ScheduledTime,
                        IsGiven    = log?.GivenTime != null
                    };
                })
                .ToList();
        }

        /// <summary>
        /// Returns the "work day" date for a given datetime.
        /// A work day starts at 07:00 — so anything before 07:00 belongs to the previous calendar day.
        /// Dag (07–15), Aften (15–23), and Nat (23–07) all share the same work day date,
        /// and IsGiven resets at the start of each new Dag shift.
        /// </summary>
        private static DateOnly WorkDayDate(DateTime dt) =>
            dt.Hour < 7
                ? DateOnly.FromDateTime(dt.AddDays(-1))
                : DateOnly.FromDateTime(dt);

        /// <summary>
        /// Creates PN entry DTO objects for a resident on the specified date.
        /// </summary>
        /// <param name="resident">The Resident entity containing PN information.</param>
        /// <param name="date">The date used to filter PN entries.</param>
        /// <returns>Returns a list of PNEntryDto objects.</returns>
        private static List<PNEntryDto> MapPNEntries(Resident resident, DateTime date)
        {
            return resident.PNs
                .Where(pn => WorkDayDate(pn.PNGivenTime) == WorkDayDate(date))
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

        // ── Shiftboard resident-card update ───────────────────────────────────────

        /// <summary>
        /// Persists the five editable fields on a resident card:
        ///   Status note, Risk level, Assigned staff, Medicine IsGiven toggles, and new PN entries.
        /// Called from the /shiftboard_viewonly page when staff save a card.
        /// </summary>
        public async Task<bool> UpdateResidentCardAsync(ResidentCardDto dto, CancellationToken ct = default)
        {
            // Guard: we need at least a valid ResidentID to do anything useful
            if (dto.ResidentID == Guid.Empty || dto.ShiftBoardID == Guid.Empty) return false;

            // ── 1. Status note + Risk level (only when a status record exists) ──
            if (dto.ResidentStatusID != Guid.Empty)
            {
                var status = await _context.ResidentStatuses
                    .Include(rs => rs.StaffResidentStatuses)
                    .FirstOrDefaultAsync(rs => rs.ResidentStatusID == dto.ResidentStatusID, ct);

                if (status is not null)
                {
                    status.Status = dto.LatestStatusNote ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(dto.RiskLevel))
                    {
                        var riskLevel = await _context.RiskLevels
                            .FirstOrDefaultAsync(rl => rl.RiskLevelName == dto.RiskLevel, ct);
                        if (riskLevel is not null)
                            status.RiskLevelID = riskLevel.RiskLevelID;
                    }

                    // ── 2. Assigned staff (scoped to shiftboard) ──
                    // EF propagates composite-PK FK values from the navigation property, not the raw FK field.
                    // Load ShiftBoard with tracking and set it explicitly on each new entity.
                    var shiftBoard = await _context.ShiftBoards
                        .FirstOrDefaultAsync(sb => sb.ShiftBoardID == dto.ShiftBoardID, ct);
                    if (shiftBoard is null) return false;

                    var existingStaffIds = status.StaffResidentStatuses
                        .Where(srs => srs.ShiftBoardID == dto.ShiftBoardID)
                        .Select(srs => srs.StaffID)
                        .ToHashSet();

                    var incomingStaff = dto.AssignedStaff.Count > 0
                        ? await _context.Staffs
                            .Where(s => dto.AssignedStaff.Contains(s.StaffName))
                            .ToListAsync(ct)
                        : new List<Staff>();

                    var incomingStaffIds = incomingStaff.Select(s => s.StaffID).ToHashSet();

                    var toRemove = status.StaffResidentStatuses
                        .Where(srs => srs.ShiftBoardID == dto.ShiftBoardID && !incomingStaffIds.Contains(srs.StaffID))
                        .ToList();
                    _context.StaffResidentStatuses.RemoveRange(toRemove);

                    foreach (var staff in incomingStaff.Where(s => !existingStaffIds.Contains(s.StaffID)))
                    {
                        _context.StaffResidentStatuses.Add(new StaffResidentStatus
                        {
                            StaffID          = staff.StaffID,
                            ResidentStatusID = status.ResidentStatusID,
                            ShiftBoard       = shiftBoard,
                            AssignedAt       = DateTime.Now
                        });
                    }
                }
            }

            // ── 3. Medicine IsGiven toggles (always runs, independent of status) ──
            var shiftDate = WorkDayDate(dto.Date);

            foreach (var medDto in dto.MedicineSchedule)
            {
                var medicine = await _context.Medicines
                    .Include(m => m.MedicineLogs.Where(ml => ml.Date == shiftDate))
                    .FirstOrDefaultAsync(m => m.MedicineID == medDto.MedicineID, ct);

                if (medicine is null) continue;

                var log = medicine.MedicineLogs.FirstOrDefault(ml => ml.Date == shiftDate);

                if (medDto.IsGiven && log is null)
                {
                    _context.MedicineLogs.Add(new MedicineLog
                    {
                        MedicineLogID    = Guid.NewGuid(),
                        Date             = shiftDate,
                        GivenTime        = DateTime.Now,
                        RegisteredTime   = DateTime.Now,
                        MedicineID       = medicine.MedicineID
                    });
                }
                else if (!medDto.IsGiven && log is not null)
                {
                    _context.MedicineLogs.Remove(log);
                }
            }

            // ── 4. New PN entries (append-only, deduplicated by time+med+reason) ──
            var workDayStart = shiftDate.ToDateTime(new TimeOnly(7, 0));
            var workDayEnd   = shiftDate.AddDays(1).ToDateTime(new TimeOnly(7, 0));
            var existingPNs = await _context.PNs
                .Where(pn => pn.ResidentID == dto.ResidentID
                          && pn.PNGivenTime >= workDayStart
                          && pn.PNGivenTime < workDayEnd)
                .ToListAsync(ct);

            foreach (var pnDto in dto.PNEntries)
            {
                var givenTime = ParsePNDateTime(pnDto.TimeOfAdministration, dto.Date);

                var alreadyExists = existingPNs.Any(pn =>
                    pn.PNGivenTime.Hour   == givenTime.Hour   &&
                    pn.PNGivenTime.Minute == givenTime.Minute &&
                    pn.PNMedication       == pnDto.Medication &&
                    pn.PNReason           == pnDto.Reason);

                if (alreadyExists) continue;

                var newPN = new PN
                {
                    PNID              = Guid.NewGuid(),
                    PNMedication      = pnDto.Medication,
                    PNGivenTime       = givenTime,
                    PNReason          = pnDto.Reason,
                    PNRegisteredTime  = DateTime.Now,
                    ResidentID        = dto.ResidentID
                };
                _context.PNs.Add(newPN);

                if (!string.IsNullOrWhiteSpace(pnDto.IssuedBy))
                {
                    var staff = await _context.Staffs
                        .FirstOrDefaultAsync(s => s.StaffName == pnDto.IssuedBy, ct);
                    if (staff is not null)
                    {
                        _context.StaffPNs.Add(new StaffPN
                        {
                            StaffID = staff.StaffID,
                            PNID    = newPN.PNID
                        });
                    }
                }
            }

            await _context.SaveChangesAsync(ct);
            return true;
        }

        private static DateTime ParsePNDateTime(string timeStr, DateTime date)
        {
            if (TimeOnly.TryParse(timeStr, out var t))
                return date.Date.AddHours(t.Hour).AddMinutes(t.Minute);
            return date.Date;
        }
    }
}
