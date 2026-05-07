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

        /// <summary>
        /// Gets all PN assignments given by staff members and returns them as a list of StaffPNDTO objects.
        /// </summary>
        /// <returns>Returns a list of StaffPNDTO objects.</returns>
        public async Task<IEnumerable<StaffPNDTO>> GetAllStaffPNsAsync()
        {
            return await _context.StaffPNs
                .AsNoTracking()
                .Select(MapToDtoExpression())
                .OrderBy(p => p.StaffName)
                .ToListAsync();
        }

        /// <summary>
        /// Function to update the binding between PN and Staff, so that you can see who has given what to whom.
        /// <remarks>If the staff member does not exist, returns false.</remarks>
        /// </summary>
        /// <param name="dto">DTO object containing PN and staff information.</param>
        /// <returns>Returns true if the update is successful, otherwise false.</returns>
        public async Task<bool> PostStaffPNAsync(StaffPNDTO dto)
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

        /// <summary>
        /// Maps DTO to Entity using LINQ, it provides better performance as there are no joins needed.
        /// </summary>
        /// <returns>Returns a LINQ expression that can be used to map StaffPN to StaffPNDTO.</returns>
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
