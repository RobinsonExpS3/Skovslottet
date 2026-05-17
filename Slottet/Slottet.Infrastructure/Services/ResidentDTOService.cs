using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class ResidentDTOService : IResidentDTOService
    {
        private readonly SlottetDBContext _context;

        public ResidentDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sends a query to the database to retrieve all active resident objects and maps them to DTO objects.
        /// </summary>
        /// <returns>Returns a list of EditResidentDTO objects.</returns>
        public async Task<IEnumerable<EditResidentDTO>> GetAllResidentsAsync()
        {
            var residents = await _context.Residents
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.ResidentID)
                .Select(MapToDtoExpression())
                .ToListAsync();

            var groceryDays = await GetGroceryDayLookupAsync();
            var paymentMethods = await GetPaymentMethodsLookupAsync();

            foreach (var resident in residents)
            {
                resident.GroceryDays = groceryDays;
                resident.PaymentMethods = paymentMethods;
            }

            return residents;
        }

        /// <summary>
        /// Sends a query to the database to retrieve a resident object based on a specific ID and maps it to a DTO object.
        /// </summary>
        /// <param name="id">The ID of the resident to retrieve.</param>
        /// <returns>Returns an EditResidentDTO object if found, otherwise null.</returns>
        public async Task<EditResidentDTO?> GetResidentByIdAsync(Guid id)
        {
            var residentDto = await _context.Residents
                .AsNoTracking()
                .Where(r => r.ResidentID == id)
                .Select(MapToDtoExpression())
                .FirstOrDefaultAsync();

            if (residentDto == null)
            {
                return null;
            }

            residentDto.PaymentMethodIDs = await _context.ResidentPaymentMethods
                .AsNoTracking()
                .Where(rpm => rpm.ResidentID == id)
                .Select(rpm => rpm.PaymentMethodID)
                .ToListAsync();

            residentDto.MedicineTimes = await _context.Medicines
                .AsNoTracking()
                .Where(m => m.ResidentID == id)
                .Select(m => m.ScheduledTime)
                .ToListAsync();

            residentDto.GroceryDays = await GetGroceryDayLookupAsync();
            residentDto.PaymentMethods = await GetPaymentMethodsLookupAsync();

            return residentDto;
        }

        /// <summary>
        /// Gets all grocery day lookup values used when editing residents.
        /// </summary>
        /// <returns>Returns a list of ResidentLookupDTO objects.</returns>
        public Task<List<ResidentLookupDTO>> GetResidentGroceryDaysAsync() => GetGroceryDayLookupAsync();

        /// <summary>
        /// Gets all payment method lookup values used when editing residents.
        /// </summary>
        /// <returns>Returns a list of ResidentLookupDTO objects.</returns>
        public Task<List<ResidentLookupDTO>> GetResidentPaymentMethodsAsync() => GetPaymentMethodsLookupAsync();

        /// <summary>
        /// Creates a new resident object in the database based on the EditResidentDTO object.
        /// </summary>
        /// <param name="dto">DTO object containing resident information.</param>
        /// <returns>Returns the created EditResidentDTO object.</returns>
        public async Task<EditResidentDTO?> PostResidentAsync(EditResidentDTO dto)
        {
            var maxSortOrder = await _context.Residents.AnyAsync()
                ? await _context.Residents.MaxAsync(r => r.SortOrder)
                : -1;

            var resident = new Resident
            {
                ResidentID = dto.ResidentID == Guid.Empty ? Guid.NewGuid() : dto.ResidentID,
                ResidentName = dto.ResidentName,
                IsActive = dto.IsActive,
                GroceryDayID = dto.GroceryDayID,
                SortOrder = maxSortOrder + 1
            };

            _context.Residents.Add(resident);

            AddPaymentMethods(resident.ResidentID, dto.PaymentMethodIDs);
            AddMedicines(resident.ResidentID, dto.MedicineTimes);

            await _context.SaveChangesAsync();

            var createdResident = await GetResidentByIdAsync(resident.ResidentID);
            return createdResident;
        }

        /// <summary>
        /// Updates a resident object in the database based on the provided ID and EditResidentDTO object.
        /// </summary>
        /// <param name="id">The ID of the resident to update.</param>
        /// <param name="dto">DTO object containing updated resident information.</param>
        /// <returns>Returns true if the update is successful, otherwise false.</returns>
        public async Task<bool> PutResidentAsync(Guid id, EditResidentDTO dto)
        {
            var existingResident = await _context.Residents
                .FirstOrDefaultAsync(r => r.ResidentID == id);

            if (existingResident == null)
            {
                return false;
            }

            existingResident.ResidentName = dto.ResidentName;
            existingResident.IsActive = dto.IsActive;
            existingResident.GroceryDayID = dto.GroceryDayID;

            var existingPaymentMethods = await _context.ResidentPaymentMethods
                .Where(rpm => rpm.ResidentID == id)
                .ToListAsync();
            _context.ResidentPaymentMethods.RemoveRange(existingPaymentMethods);
            AddPaymentMethods(id, dto.PaymentMethodIDs);

            // Soft delete eksisterende medicin-tider (bevarer MedicineLog historik) og tilføj nye.
            var existingMedicines = await _context.Medicines
                .Where(m => m.ResidentID == id)
                .ToListAsync();
            foreach (var medicine in existingMedicines)
            {
                medicine.IsActive = false;
            }
            AddMedicines(id, dto.MedicineTimes);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Soft-deletes a resident based on the given ID. Bevarer al relateret data
        /// (PNs, Medicines, ResidentStatuses) for sporbarhed og dokumentationskrav.
        /// </summary>
        /// <param name="id">The ID of the resident to delete.</param>
        /// <returns>Returns true if the deletion is successful, otherwise false.</returns>
        public async Task<bool> DeleteResidentAsync(Guid id)
        {
            return await DeactivateResidentAsync(id);
        }

        /// <summary>
        /// Soft-deletes a resident by setting IsActive = false.
        /// Leaves all related data (PNs, statuses etc.) intact.
        /// </summary>
        public async Task<bool> DeactivateResidentAsync(Guid id, CancellationToken ct = default)
        {
            var resident = await _context.Residents
                .FirstOrDefaultAsync(r => r.ResidentID == id, ct);

            if (resident == null)
                return false;

            resident.IsActive = false;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Gets all grocery day lookup values and maps them to ResidentLookupDTO objects.
        /// </summary>
        /// <returns>Returns a list of ResidentLookupDTO objects.</returns>
        private async Task<List<ResidentLookupDTO>> GetGroceryDayLookupAsync()
        {
            return await _context.GroceryDays
                .AsNoTracking()
                .OrderBy(g => g.GroceryDayName)
                .Select(g => new ResidentLookupDTO
                {
                    ID = g.GroceryDayID,
                    Name = g.GroceryDayName
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all payment method lookup values and maps them to ResidentLookupDTO objects.
        /// </summary>
        /// <returns>Returns a list of ResidentLookupDTO objects.</returns>
        private async Task<List<ResidentLookupDTO>> GetPaymentMethodsLookupAsync()
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .OrderBy(p => p.PaymentMethodName)
                .Select(p => new ResidentLookupDTO
                {
                    ID = p.PaymentMethodID,
                    Name = p.PaymentMethodName
                })
                .ToListAsync();
        }

        /// <summary>
        /// Adds relation rows between a resident and the selected payment methods.
        /// </summary>
        /// <param name="residentID">The ID of the resident to attach payment methods to.</param>
        /// <param name="paymentMethodsIDs">The IDs of the payment methods to add.</param>
        private void AddPaymentMethods(Guid residentID, List<Guid>? paymentMethodsIDs)
        {
            if (paymentMethodsIDs == null || paymentMethodsIDs.Count == 0)
            {
                return;
            }

            var relationRows = paymentMethodsIDs
                .Distinct()
                .Select(paymentMethodID => new ResidentPaymentMethod
                {
                    ResidentID = residentID,
                    PaymentMethodID = paymentMethodID,
                });

            _context.ResidentPaymentMethods.AddRange(relationRows);
        }

        /// <summary>
        /// Adds medicine rows for a resident based on the selected medicine times.
        /// </summary>
        /// <param name="residentID">The ID of the resident to attach medicines to.</param>
        /// <param name="medicineTimes">The medicine times to add.</param>
        private void AddMedicines(Guid residentID, List<TimeOnly>? medicineTimes)
        {
            if (medicineTimes == null || medicineTimes.Count == 0)
            {
                return;
            }

            var medicineRows = medicineTimes.Select(scheduledTime => new Medicine
            {
                MedicineID    = Guid.NewGuid(),
                ResidentID    = residentID,
                ScheduledTime = scheduledTime,
            });

            _context.Medicines.AddRange(medicineRows);
        }

        /// <summary>
        /// Creates an expression that maps a Resident entity to an EditResidentDTO for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of Resident entities to EditResidentDTO objects.</remarks>
        /// <returns>An expression that projects a Resident object into an EditResidentDTO instance.</returns>
        /// <summary>
        /// Returns all active residents mapped to ResidentCardDto — same view as ShiftBoard.
        /// </summary>
        public async Task<bool> SwapResidentSortOrderAsync(Guid residentIdA, Guid residentIdB, CancellationToken ct = default)
        {
            var a = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentID == residentIdA, ct);
            var b = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentID == residentIdB, ct);

            if (a is null || b is null) return false;

            (a.SortOrder, b.SortOrder) = (b.SortOrder, a.SortOrder);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> UpdateGroceryDayAsync(Guid residentId, Guid groceryDayId, CancellationToken ct = default)
        {
            var resident = await _context.Residents
                .FirstOrDefaultAsync(r => r.ResidentID == residentId, ct);

            if (resident is null) return false;

            resident.GroceryDayID = groceryDayId;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> UpdateMedicineTimesAsync(Guid residentId, List<TimeOnly> times, CancellationToken ct = default)
        {
            var exists = await _context.Residents.AnyAsync(r => r.ResidentID == residentId, ct);
            if (!exists) return false;

            var existing = await _context.Medicines
                .Where(m => m.ResidentID == residentId)
                .ToListAsync(ct);

            // Soft delete tider der ikke længere er i listen (bevarer MedicineLog historik)
            foreach (var medicine in existing.Where(m => !times.Contains(m.ScheduledTime)))
            {
                medicine.IsActive = false;
            }

            // Tilføj kun helt nye tider
            var existingTimes = existing.Select(m => m.ScheduledTime).ToHashSet();
            var toAdd = times.Where(t => !existingTimes.Contains(t)).ToList();
            AddMedicines(residentId, toAdd);

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> UpdatePaymentMethodsAsync(Guid residentId, List<Guid> paymentMethodIds, CancellationToken ct = default)
        {
            var exists = await _context.Residents.AnyAsync(r => r.ResidentID == residentId, ct);
            if (!exists) return false;

            var existing = await _context.ResidentPaymentMethods
                .Where(rpm => rpm.ResidentID == residentId)
                .ToListAsync(ct);

            _context.ResidentPaymentMethods.RemoveRange(existing);
            AddPaymentMethods(residentId, paymentMethodIds);

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<List<ResidentCardDto>> GetResidentCardsAsync(CancellationToken ct = default)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var residents = await _context.Residents
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.ResidentID)
                .Include(r => r.GroceryDay)
                .Include(r => r.Medicines)
                    .ThenInclude(m => m.MedicineLogs.Where(ml => ml.Date == today))
                .Include(r => r.PNs.Where(pn => pn.PNGivenTime.Date == DateTime.Today))
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

            return residents.Select(r => MapToResidentCard(r, today)).ToList();
        }

        private static ResidentCardDto MapToResidentCard(Resident resident, DateOnly today)
        {
            var latestStatus = resident.ResidentStatuses
                .OrderByDescending(s => s.Date)
                .FirstOrDefault();

            return new ResidentCardDto
            {
                ResidentStatusID  = latestStatus?.ResidentStatusID ?? Guid.Empty,
                ResidentID        = resident.ResidentID,
                Date              = today.ToDateTime(TimeOnly.MinValue),
                ResidentName      = resident.ResidentName,
                IsActive          = resident.IsActive,
                RiskLevel         = latestStatus?.RiskLevel?.RiskLevelName,
                LatestStatusNote  = latestStatus?.Status,
                GroceryDay        = resident.GroceryDay?.GroceryDayName,
                PaymentMethod     = resident.ResidentPaymentMethods
                                        .Select(rpm => rpm.PaymentMethod?.PaymentMethodName)
                                        .FirstOrDefault(),
                PaymentMethodIDs  = resident.ResidentPaymentMethods
                                        .Select(rpm => rpm.PaymentMethodID)
                                        .ToList(),
                AssignedStaff     = latestStatus?.StaffResidentStatuses
                                        .Select(srs => srs.Staff.StaffName)
                                        .OrderBy(n => n)
                                        .ToList() ?? new(),
                MedicineSchedule  = resident.Medicines
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
                                        .ToList(),
                PNEntries         = resident.PNs
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
                                        .ToList()
            };
        }

        private static Expression<Func<Resident, EditResidentDTO>> MapToDtoExpression()
        {
            return resident => new EditResidentDTO
            {
                ResidentID = resident.ResidentID,
                ResidentName = resident.ResidentName,
                IsActive = resident.IsActive,
                GroceryDayID = resident.GroceryDayID
            };
        }
    }
}
