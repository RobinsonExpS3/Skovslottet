using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class SpecialResponsibilityDTOService : ISpecialResponsibilityDTOService
    {
        private readonly SlottetDBContext _context;

        public SpecialResponsibilityDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Send a query to the database to retrieve all SpecialResponsibility objects, thats orderes by task name
        /// and then map those to a list of SpecialResponsibilityEntryDto objects, which is returned as an IEnumerable.
        /// </summary>
        /// <returns>Returns an IEnumerable of SpecialResponsibilityEntryDto objects.</returns>
        public async Task<IEnumerable<SpecialResponsibilityEntryDto>> GetAllSpecialResponsibilitiesAsync()
        {
            return await _context.SpecialResponsibilities
                .AsNoTracking()
                .OrderBy(sr => sr.TaskName)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }

        /// <summary>
        /// Send a query to the database, to retrieve a single SpecialResponsibility object based on the provided id,
        /// and then map that object to a SpecialResponsibilityEntryDto object.
        /// </summary>
        /// <param name="id">The ID of the SpecialResponsibility to retrieve.</param>
        /// <returns>Returns a SpecialResponsibilityEntryDto object if found, otherwise null.</returns>

        public async Task<SpecialResponsibilityEntryDto?> GetSpecialResponsibilityByIdAsync(Guid id)
        {
            return await _context.SpecialResponsibilities
                .AsNoTracking()
                .Where(sr => sr.SpecialResponsibilityID == id)
                .Select(MapToDtoExpression())
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new SpecialResposibility object based on the provided SpecialResponsibilityEntryDto, 
        /// adds it to the database, and then returns the created object as a SpecialResponsibilityEntryDto.
        /// </summary>
        /// <param name="dto">DTO object containing the specialresponsibility information</param>
        /// <returns>Returns the created Specialresponsibility</returns>
        public async Task<SpecialResponsibilityEntryDto?> PostSpecialResponsibilityAsync(SpecialResponsibilityEntryDto dto)
        {
            var specialResponsibility = new SpecialResponsibility
            {
                SpecialResponsibilityID = dto.SpecialResponsibilityID == Guid.Empty
                    ? Guid.NewGuid()
                    : dto.SpecialResponsibilityID,
                TaskName = dto.Description,
            };

            _context.SpecialResponsibilities.Add(specialResponsibility);
            await _context.SaveChangesAsync();

            var createdSpecialResponsibility = await GetSpecialResponsibilityByIdAsync(specialResponsibility.SpecialResponsibilityID);
            return createdSpecialResponsibility;
        }

        /// <summary>
        /// Asynchronously updates the special responsibility entry identified by the specified ID with the values
        /// provided in the given DTO.
        /// </summary>
        /// <param name="id">The unique identifier of the special responsibility entry to update.</param>
        /// <param name="dto">The data transfer object containing the updated values 
        /// for the special responsibility entry.</param>
        /// <returns>Returns true if the update was successful, otherwise false.</returns>
        public async Task<bool> PutSpecialResponsibilityAsync(Guid id, SpecialResponsibilityEntryDto dto)
        {
            var existingSpecialResponsibility = await _context.SpecialResponsibilities
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);

            if (existingSpecialResponsibility == null)
            {
                return false;
            }

            existingSpecialResponsibility.TaskName = dto.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deleted a staff object from the database, based on the given ID.
        /// </summary>
        /// <param name="id">The ID of the SpecialResponsibility to delete.</param>
        /// <returns>Returns true if the deletion is successful, otherwise false.</returns>
        public async Task<bool> DeleteSpecialResponsibilityAsync(Guid id)
        {
            var specialResponsibility = await _context.SpecialResponsibilities
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);

            if (specialResponsibility == null)
            {
                return false;
            }

            var staffAssignments = await _context.Set<SpecialResponsibilityStaff>()
                .Where(srs => srs.SpecialResponsibilityID == id)
                .ToListAsync();
            _context.Set<SpecialResponsibilityStaff>().RemoveRange(staffAssignments);

            _context.SpecialResponsibilities.Remove(specialResponsibility);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Creates an expression that maps a SpecialResponsibility entity to a SpecialResponsibilityEntryDto 
        /// for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of SpecialResponsibility entities 
        /// to SpecialResponsibilityEntryDto objects.
        /// </remarks>
        /// <returns>An expression that projects a SpecialResponsibility object into a SpecialResponsibilityEntryDto instance.</returns>
        private static Expression<Func<SpecialResponsibility, SpecialResponsibilityEntryDto>> MapToDtoExpression()
        {
            return specialResponsibility => new SpecialResponsibilityEntryDto
            {
                SpecialResponsibilityID = specialResponsibility.SpecialResponsibilityID,
                Description = specialResponsibility.TaskName,
                StaffName = specialResponsibility.SpecialResponsibilityStaffs
                    .OrderByDescending(srs => srs.AssignedAt)
                    .Select(srs => srs.Staff.StaffName)
                    .FirstOrDefault() ?? "Unassigned",
                StaffInitials = specialResponsibility.SpecialResponsibilityStaffs
                    .OrderByDescending(srs => srs.AssignedAt)
                    .Select(srs => srs.Staff.Initials)
                    .FirstOrDefault() ?? string.Empty
            };
        }
    }
}
