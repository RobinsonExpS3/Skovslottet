using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System.Linq.Expressions;

namespace Slottet.Infrastructure.Services
{
    public class SwapPhoneDTOService : ISwapPhoneDTOService
    {
        private readonly SlottetDBContext _context;

        public SwapPhoneDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SwapPhoneRecDTO>> GetAllAsync()
        {
            return await _context.Phones
                .AsNoTracking()
                .Select(MapToDtoExpression())
                .OrderBy(p => p.PhoneNumber)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(SwapPhoneRecDTO dto)
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
                AssignedAt = DateTime.Now
            };

            _context.StaffPhones.Add(assignment);
            await _context.SaveChangesAsync();

            return true;
        }

        private static Expression<Func<Phone, SwapPhoneRecDTO>> MapToDtoExpression()
        {
            return phone => new SwapPhoneRecDTO(
                phone.PhoneID,
                phone.PhoneNumber,
                phone.StaffPhones
                    .OrderByDescending(sp => sp.AssignedAt)
                    .Select(sp => sp.StaffID)
                    .FirstOrDefault(),
                phone.StaffPhones
                    .OrderByDescending(sp => sp.AssignedAt)
                    .Select(sp => sp.Staff.StaffName)
                    .FirstOrDefault(),
                phone.StaffPhones
                    .OrderByDescending(sp => sp.AssignedAt)
                    .Select(sp => (DateTime?)sp.AssignedAt)
                    .FirstOrDefault()
            );
        }
    }
}