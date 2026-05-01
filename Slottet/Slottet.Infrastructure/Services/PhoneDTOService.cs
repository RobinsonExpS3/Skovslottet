using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class PhoneDTOService : IPhoneDTOService
    {
        private readonly SlottetDBContext _context;

        public PhoneDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sends a query to the database to retrieve all phone objects.
        /// </summary>
        /// <returns>Returns a list of Phone objects.</returns>
        public async Task<IEnumerable<PhoneDTO>> GetAllPhonesAsync()
        {
            return await _context.Phones
                .AsNoTracking()
                .OrderBy(p => p.PhoneNumber)
                .Select(phone => new PhoneDTO
                {
                    PhoneID = phone.PhoneID,
                    PhoneNumber = phone.PhoneNumber,
                    DepartmentID = phone.DepartmentID
                })
                .ToListAsync();
        }

        /// <summary>
        /// Sends a query to the database to retrieve a phone object based on a specific ID.
        /// </summary>
        /// <param name="id">The ID of the phone to retrieve.</param>
        /// <returns>Returns a Phone object if found, otherwise null.</returns>
        public async Task<PhoneDTO?> GetPhoneByIdAsync(Guid id)
        {
            return await _context.Phones
                .AsNoTracking()
                .Where(p => p.PhoneID == id)
                .Select(phone => new PhoneDTO
                {
                    PhoneID = phone.PhoneID,
                    PhoneNumber = phone.PhoneNumber,
                    DepartmentID = phone.DepartmentID
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new phone object in the database.
        /// </summary>
        /// <param name="dto">DTO object containing phone information.</param>
        /// <returns>Returns the created Phone object.</returns>
        public async Task<PhoneDTO?> PostPhoneAsync(PhoneDTO dto)
        {
            var phone = new Phone
            {
                PhoneID = dto.PhoneID == Guid.Empty ? Guid.NewGuid() : dto.PhoneID,
                PhoneNumber = dto.PhoneNumber,
                DepartmentID = dto.DepartmentID
            };

            _context.Phones.Add(phone);
            await _context.SaveChangesAsync();

            var createdPhone = await GetPhoneByIdAsync(phone.PhoneID);
            return createdPhone;
        }

        /// <summary>
        /// Updates a phone object in the database based on the provided ID and Phone object.
        /// </summary>
        /// <param name="id">The ID of the phone to update.</param>
        /// <param name="dto">DTO object containing updated phone information.</param>
        /// <returns>Returns true if the update is successful, otherwise false.</returns>
        public async Task<bool> PutPhoneAsync(Guid id, PhoneDTO dto)
        {
            var existingPhone = await _context.Phones
                .FirstOrDefaultAsync(p => p.PhoneID == id);

            if (existingPhone == null)
            {
                return false;
            }

            existingPhone.PhoneNumber = dto.PhoneNumber;
            existingPhone.DepartmentID = dto.DepartmentID;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a phone object from the database based on the given ID.
        /// </summary>
        /// <param name="id">The ID of the phone to delete.</param>
        /// <returns>Returns true if the deletion is successful, otherwise false.</returns>
        public async Task<bool> DeletePhoneAsync(Guid id)
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
