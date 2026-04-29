using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System.Linq.Expressions;

namespace Slottet.Infrastructure.Services
{
    public class MedicineDTOService : IMedicineDTOService
    {
        private readonly SlottetDBContext _context;

        public MedicineDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicineAPI_DTO>> GetAllAsync()
        {
            return await _context.Medicines
                .AsNoTracking()
                .OrderBy(m => m.ResidentID)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }

        public async Task<MedicineAPI_DTO?> GetByIdAsync(Guid id)
        {
            return await _context.Medicines
                .AsNoTracking()
                .Where(m => m.MedicineID == id)
                .Select(MapToDtoExpression())
                .FirstOrDefaultAsync();
        }

        public async Task<MedicineAPI_DTO> CreateAsync(MedicineAPI_DTO dto)
        {
            var medicine = new Medicine
            {
                MedicineID = Guid.NewGuid(),
                ResidentID = dto.ResidentID,
                MedicineTime = dto.MedicineTime,
                MedicineGivenTime = dto.MedicineGivenTime ?? DateTime.Now,
                MedicineRegisteredTime = null
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return MapToDto(medicine);
        }

        public async Task<bool> UpdateAsync(Guid id, MedicineAPI_DTO dto)
        {
            var existingMedicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.MedicineID == id);

            if (existingMedicine is null)
            {
                return false;
            }

            existingMedicine.ResidentID = dto.ResidentID;
            existingMedicine.MedicineTime = dto.MedicineTime;
            existingMedicine.MedicineGivenTime = dto.MedicineGivenTime ?? existingMedicine.MedicineGivenTime;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.MedicineID == id);

            if (medicine is null)
            {
                return false;
            }

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();
            return true;
        }

        private static Expression<Func<Medicine, MedicineAPI_DTO>> MapToDtoExpression()
        {
            return medicine => new MedicineAPI_DTO
            {
                MedicineID = medicine.MedicineID,
                ResidentID = medicine.ResidentID,
                MedicineTime = medicine.MedicineTime,
                MedicineGivenTime = medicine.MedicineGivenTime,
                MedicineRegisteredTime = medicine.MedicineRegisteredTime
            };
        }

        private static MedicineAPI_DTO MapToDto(Medicine medicine)
        {
            return new MedicineAPI_DTO
            {
                MedicineID = medicine.MedicineID,
                ResidentID = medicine.ResidentID,
                MedicineTime = medicine.MedicineTime,
                MedicineGivenTime = medicine.MedicineGivenTime,
                MedicineRegisteredTime = medicine.MedicineRegisteredTime
            };
        }
    }
}
