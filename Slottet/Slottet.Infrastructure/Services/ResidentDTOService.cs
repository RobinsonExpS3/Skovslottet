using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Infrastructure.Services {
    public class ResidentDTOService : IResidentDTOService {
        private readonly SlottetDBContext _context;

        public ResidentDTOService(SlottetDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<EditResidentDTO>> GetAllAsync() {
            var residents = await _context.Residents
                .AsNoTracking()
                .OrderBy(r => r.ResidentID)
                .Select(MapToDtoExpression())
                .ToListAsync();

            var groceryDays = await GetGroceryDayLookupAsync();
            var paymentMethods = await GetPaymentMethodsLookupAsync();

            foreach(var resident in residents) {
                resident.GroceryDays = groceryDays;
                resident.PaymentMethods = paymentMethods;
            }

            return residents;
        }

        public async Task<EditResidentDTO?> GetByIdAsync(Guid id) {
            var residentDto = await _context.Residents
                .AsNoTracking()
                .Where(r => r.ResidentID == id)
                .Select(MapToDtoExpression())
                .FirstOrDefaultAsync();

            if(residentDto == null) {
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

        public Task<List<ResidentLookupDTO>> GetGroceryDaysAsync() => GetGroceryDayLookupAsync();
        public Task<List<ResidentLookupDTO>> GetPaymentMethodsAsync() => GetPaymentMethodsLookupAsync();

        public async Task<EditResidentDTO> CreateAsync(EditResidentDTO dto) {
            var resident = new Resident {
                ResidentID = dto.ResidentID == Guid.Empty ? Guid.NewGuid() : dto.ResidentID,
                ResidentName = dto.ResidentName,
                IsActive = dto.IsActive,
                GroceryDayID = dto.GroceryDayID
            };

            _context.Residents.Add(resident);

            AddPaymentMethods(resident.ResidentID, dto.PaymentMethodIDs);
            AddMedicines(resident.ResidentID, dto.MedicineTimes);

            await _context.SaveChangesAsync();

            var createdResident = await GetByIdAsync(resident.ResidentID);
            return createdResident ?? MapToDTO(resident);
        }

        public async Task<bool> UpdateAsync(Guid id, EditResidentDTO dto) {
            var existingResident = await _context.Residents
                .FirstOrDefaultAsync(r => r.ResidentID == id);

            if(existingResident == null) {
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

        public async Task<bool> DeleteAsync(Guid id) {
            var existingResident = await _context.Residents
                .FirstOrDefaultAsync(r => r.ResidentID == id);

            if (existingResident == null) {
                return false;
            }

            var residentStatusIDs = await _context.ResidentStatuses
                .Where(rs => rs.ResidentID == id)
                .Select(rs => rs.ResidentStatusID)
                .ToListAsync();

            if (residentStatusIDs.Count > 0) {
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

        private async Task<List<ResidentLookupDTO>> GetGroceryDayLookupAsync() {
            return await _context.GroceryDays
                .AsNoTracking()
                .OrderBy(g => g.GroceryDayName)
                .Select(g => new ResidentLookupDTO {
                    ID = g.GroceryDayID,
                    Name = g.GroceryDayName
                })
                .ToListAsync();
        }

        private async Task<List<ResidentLookupDTO>> GetPaymentMethodsLookupAsync() {
            return await _context.PaymentMethods
                .AsNoTracking()
                .OrderBy(p => p.PaymentMethodName)
                .Select(p => new ResidentLookupDTO {
                    ID = p.PaymentMethodID,
                    Name = p.PaymentMethodName
                })
                .ToListAsync();
        }

        private void AddPaymentMethods(Guid residentID, List<Guid>? paymentMethodsIDs) {
            if (paymentMethodsIDs == null || paymentMethodsIDs.Count == 0) {
                return;
            }

            var relationRows = paymentMethodsIDs
                .Distinct()
                .Select(paymentMethodID => new ResidentPaymentMethod {
                    ResidentID = residentID,
                    PaymentMethodID = paymentMethodID,
                });

            _context.ResidentPaymentMethods.AddRange(relationRows);
        }

        private void AddMedicines(Guid residentID, List<DateTime>? medicineTimes) {
            if (medicineTimes == null || medicineTimes.Count == 0) {
                return;
            }

            var medicineRows = medicineTimes.Select(medicineTime => new Medicine {
                MedicineID = Guid.NewGuid(),
                ResidentID = residentID,
                MedicineTime = medicineTime,
                MedicineGivenTime = DateTime.Now,
                MedicineRegisteredTime = DateTime.Now
            });

            _context.Medicines.AddRange(medicineRows);
        }

        private static System.Linq.Expressions.Expression<Func<Resident, EditResidentDTO>> MapToDtoExpression() {
            return resident => new EditResidentDTO {
                ResidentID = resident.ResidentID,
                ResidentName = resident.ResidentName,
                IsActive = resident.IsActive,
                GroceryDayID = resident.GroceryDayID
            };
        }

        private static EditResidentDTO MapToDTO(Resident resident) {
            return new EditResidentDTO {
                ResidentID = resident.ResidentID,
                ResidentName = resident.ResidentName,
                IsActive = resident.IsActive,
                GroceryDayID = resident.GroceryDayID
            };
        }
    }
}