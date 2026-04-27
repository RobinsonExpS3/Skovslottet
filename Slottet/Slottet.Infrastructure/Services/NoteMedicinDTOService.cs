using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class NoteMedicinDTOService : INoteMedicineDTOService
    {
        private readonly SlottetDBContext _context;

        public NoteMedicinDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<NoteMedicinDTO>> GetAllAsync()
        //{
        //    return await _context.Medicines
        //        .AsNoTracking()
        //        .OrderBy(m => m.ResidentID)
        //        .Select(MapToDtoExpression())
        //        .ToListAsync();
        //}

        //public async Task<NoteMedicinDTO?> GetByIdAsync(Guid id)
        //{
        //    return await _context.Medicines
        //        .AsNoTracking()
        //        .Where(m => m.MedicineID == id)
        //        .Select(MapToDtoExpression())
        //        .FirstOrDefaultAsync();
        //}

        //public async Task<NoteMedicinDTO> CreateAsync(NoteMedicinDTO dto)
        //{
        //    var medicine = new Medicine
        //    {
        //        MedicineID = Guid.NewGuid(),
        //        ResidentID = dto.ResidentID,
        //        MedicineTime = dto.MedicineTime,
        //        MedicineGivenTime = dto.MedicineGivenTime ?? DateTime.Now,
        //        MedicineRegisteredTime = DateTime.Now
        //    };

        //    _context.Medicines.Add(medicine);
        //    await _context.SaveChangesAsync();

        //    return MapToDto(medicine);
        //}

        //public async Task<bool> UpdateAsync(Guid id, NoteMedicinDTO dto)
        //{
        //    var existingMedicine = await _context.Medicines
        //        .FirstOrDefaultAsync(m => m.MedicineID == id);

        //    if (existingMedicine is null)
        //    {
        //        return false;
        //    }

        //    existingMedicine.ResidentID = dto.ResidentID;
        //    existingMedicine.MedicineTime = dto.MedicineTime;
        //    existingMedicine.MedicineGivenTime = dto.MedicineGivenTime ?? existingMedicine.MedicineGivenTime;

        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        //public async Task<bool> DeleteAsync(Guid id)
        //{
        //    var medicine = await _context.Medicines
        //        .FirstOrDefaultAsync(m => m.MedicineID == id);

        //    if (medicine is null)
        //    {
        //        return false;
        //    }

        //    _context.Medicines.Remove(medicine);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        //private static System.Linq.Expressions.Expression<Func<Medicine, NoteMedicinDTO>> MapToDtoExpression()
        //{
        //    return medicine => new NoteMedicinDTO
        //    {
        //        MedicineID = medicine.MedicineID,
        //        ResidentID = medicine.ResidentID,
        //        MedicineTime = medicine.MedicineTime,
        //        MedicineGivenTime = medicine.MedicineGivenTime,
        //        MedicineRegisteredTime = medicine.MedicineRegisteredTime
        //    };
        //}

        //private static NoteMedicinDTO MapToDto(Medicine medicine)
        //{
        //    return new NoteMedicinDTO
        //    {
        //        MedicineID = medicine.MedicineID,
        //        ResidentID = medicine.ResidentID,
        //        MedicineTime = medicine.MedicineTime,
        //        MedicineGivenTime = medicine.MedicineGivenTime,
        //        MedicineRegisteredTime = medicine.MedicineRegisteredTime
        //    };
        //}

        Task<IEnumerable<NoteMedicineDTO>> INoteMedicineDTOService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<NoteMedicineDTO?> INoteMedicineDTOService.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<NoteMedicineDTO> CreateAsync(NoteMedicineDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Guid id, NoteMedicineDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

