using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;

namespace Slottet.Infrastructure.Services
{
    public class PhoneDTOService : IPhoneDTOService
    {
        private readonly SlottetDBContext _context;

        public PhoneDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Phone>> GetAllAsync()
        {
            return await _context.Phones
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Phone?> GetByIdAsync(Guid id)
        {
            return await _context.Phones
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PhoneID == id);
        }

        public async Task<Phone> CreateAsync(Phone phone)
        {
            _context.Phones.Add(phone);
            await _context.SaveChangesAsync();
            return phone;
        }

        public async Task<bool> UpdateAsync(Guid id, Phone phone)
        {
            var existingPhone = await _context.Phones
                .FirstOrDefaultAsync(p => p.PhoneID == id);

            if (existingPhone == null)
            {
                return false;
            }

            existingPhone.PhoneNumber = phone.PhoneNumber;
            existingPhone.DepartmentID = phone.DepartmentID;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var phone = await _context.Phones
                .FirstOrDefaultAsync(p => p.PhoneID == id);

            if (phone == null)
            {
                return false;
            }

            _context.Phones.Remove(phone);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
