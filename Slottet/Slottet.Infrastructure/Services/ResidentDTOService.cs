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
                .Select(m => m.MedicineTime)
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
            var resident = new Resident
            {
                ResidentID = dto.ResidentID == Guid.Empty ? Guid.NewGuid() : dto.ResidentID,
                ResidentName = dto.ResidentName,
                IsActive = dto.IsActive,
                GroceryDayID = dto.GroceryDayID
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

            var existingMedicines = await _context.Medicines
                .Where(m => m.ResidentID == id)
                .ToListAsync();
            _context.Medicines.RemoveRange(existingMedicines);
            AddMedicines(id, dto.MedicineTimes);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a resident object and related resident data from the database based on the given ID.
        /// </summary>
        /// <param name="id">The ID of the resident to delete.</param>
        /// <returns>Returns true if the deletion is successful, otherwise false.</returns>
        public async Task<bool> DeleteResidentAsync(Guid id)
        {
            var existingResident = await _context.Residents
                .FirstOrDefaultAsync(r => r.ResidentID == id);

            if (existingResident == null)
            {
                return false;
            }

            var residentStatusIDs = await _context.ResidentStatuses
                .Where(rs => rs.ResidentID == id)
                .Select(rs => rs.ResidentStatusID)
                .ToListAsync();

            if (residentStatusIDs.Count > 0)
            {
                var staffResidentStatuses = await _context.StaffResidentStatuses
                    .Where(srs => residentStatusIDs.Contains(srs.ResidentStatusID))
                    .ToListAsync();
                _context.StaffResidentStatuses.RemoveRange(staffResidentStatuses);
            }

            var residentStatuses = await _context.ResidentStatuses
                .Where(rs => rs.ResidentID == id)
                .ToListAsync();
            _context.ResidentStatuses.RemoveRange(residentStatuses);

            var residentPaymentMethods = await _context.ResidentPaymentMethods
                .Where(rpm => rpm.ResidentID == id)
                .ToListAsync();
            _context.ResidentPaymentMethods.RemoveRange(residentPaymentMethods);

            var medicines = await _context.Medicines
                .Where(m => m.ResidentID == id)
                .ToListAsync();
            _context.Medicines.RemoveRange(medicines);

            _context.Residents.Remove(existingResident);

            await _context.SaveChangesAsync();
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
        private void AddMedicines(Guid residentID, List<DateTime>? medicineTimes)
        {
            if (medicineTimes == null || medicineTimes.Count == 0)
            {
                return;
            }

            var medicineRows = medicineTimes.Select(medicineTime => new Medicine
            {
                MedicineID = Guid.NewGuid(),
                ResidentID = residentID,
                MedicineTime = medicineTime,
                MedicineGivenTime = DateTime.Now,
                MedicineRegisteredTime = DateTime.Now
            });

            _context.Medicines.AddRange(medicineRows);
        }

        /// <summary>
        /// Creates an expression that maps a Resident entity to an EditResidentDTO for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of Resident entities to EditResidentDTO objects.</remarks>
        /// <returns>An expression that projects a Resident object into an EditResidentDTO instance.</returns>
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
