using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System.Linq.Expressions;

namespace Slottet.Infrastructure.Services
{
    public class StaffPNDTOService : IStaffPNDTOService
    {
        private readonly SlottetDBContext _context;

        public StaffPNDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffPNDTO>> GetAllAsync()
        {
            return await _context.StaffPNs
                .AsNoTracking()
                .Select(MapToDtoExpression())
                .OrderBy(p => p.StaffName)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(StaffPNDTO dto)
        {
            var staffExists = _context.Staffs.Any(s => s.StaffID == dto.StaffID);

            if (!staffExists)
            {
                return false;
            }

            var staffPNGiven = new StaffPN
            {
                StaffID = dto.StaffID,
                PNID = dto.PNID
            };

            _context.StaffPNs.Update(staffPNGiven);
            await _context.SaveChangesAsync();

            return true;
        }

        private static Expression<Func<StaffPN, StaffPNDTO>> MapToDtoExpression()
        {
            return sp => new StaffPNDTO
            {
                StaffID = sp.StaffID,
                StaffName = sp.Staff.StaffName,
                PNID = sp.PNID,
                PNName = sp.PN.PNReason
            };
        }
    }
}
