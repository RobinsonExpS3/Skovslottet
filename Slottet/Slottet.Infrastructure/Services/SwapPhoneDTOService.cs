using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class SwapPhoneDTOService : ISwapPhoneDTOService
    {
        private readonly SlottetDBContext _context;

        public SwapPhoneDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SwapPhoneDTO>> GetAllAsync()
        {
            return await _context.Phones
                .AsNoTracking()
                .Select(MapToDtoExpression())
                .OrderBy(p => p.PhoneNumber)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(SwapPhoneDTO dto)
        {

            var phoneExists = await _context.Phones.AnyAsync(p => p.PhoneID == dto.PhoneID);
            var staffExists = await _context.Staffs.AnyAsync(p => p.StaffID == dto.StaffID);

            if (!phoneExists || !staffExists)
            {
                return false;
            }

            var assignment = new StaffPhone
            {
                PhoneID = dto.PhoneID,
                StaffID = dto.StaffID,
                AssignedAt = DateTime.UtcNow
            };

            _context.StaffPhones.Add(assignment);
            await _context.SaveChangesAsync();

            return true;
        }

        private static System.Linq.Expressions.Expression<Func<Phone, SwapPhoneDTO>> MapToDtoExpression()
        {
            return phone => new SwapPhoneDTO
            {
                PhoneID = phone.PhoneID,
                PhoneNumber = phone.PhoneNumber,

                StaffID = phone.StaffPhones
                    .OrderByDescending(sp => sp.AssignedAt)
                    .Select(sp => sp.StaffID)
                    .FirstOrDefault(),

                StaffName = phone.StaffPhones
                    .OrderByDescending(sp => sp.AssignedAt)
                    .Select(sp => sp.Staff.StaffName)
                    .FirstOrDefault(),

                AssignedAt = phone.StaffPhones
                    .OrderByDescending(sp => sp.AssignedAt)
                    .Select(sp => (DateTime?)sp.AssignedAt)
                    .FirstOrDefault()
            };
        }
    }
}