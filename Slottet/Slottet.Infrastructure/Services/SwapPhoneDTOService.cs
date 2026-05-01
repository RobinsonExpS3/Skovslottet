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

        /// <summary>
        /// Finds all phone and their most recently assigned staff member asynchronously, sorted by phone number.
        /// </summary>
        /// <returns>Returns a list of SwapPhoneDTO objects.</returns>
        public async Task<IEnumerable<SwapPhoneDTO>> GetAllSwapPhonesAsync()
        {
            return await _context.Phones
                .AsNoTracking()
                .Select(MapToDtoExpression())
                .OrderBy(p => p.PhoneNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Tries to update the assignment of a phone to a staff member by creating a new entry in the StaffPhone 
        /// table, ensuring that the swap is recorded.
        /// <remarks>If the phone or staff member does not exist, returns false.</remarks>
        /// </summary>
        /// <param name="dto">DTO object containing phone and staff information.</param>
        /// <returns>Returns true if the update is successful, otherwise false.</returns>
        public async Task<bool> PostSwapPhoneAsync(SwapPhoneDTO dto)
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

        /// <summary>
        /// Maps DTO to Entity using LINQ, it provides better performance as there are no joins needed.
        /// </summary>
        /// <returns>Returns a LINQ expression that can be used to map Phone to SwapPhoneDTO.</returns>
        private static Expression<Func<Phone, SwapPhoneDTO>> MapToDtoExpression()
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
