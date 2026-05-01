using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Slottet.Infrastructure.Services
{
    public class StaffDTOService : IStaffDTOService
    {
        private readonly SlottetDBContext _context;

        public StaffDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sends a query to the database to retrieve all staff objects and maps them to the corresponding.
        /// DTO object.
        /// </summary>
        /// <returns>Returns a list of EditStaffDTO objects.</returns>
        public async Task<IEnumerable<EditStaffDTO>> GetAllStaffAsync()
        {
            return await _context.Staffs
                    .AsNoTracking()
                    .OrderBy(s => s.StaffID)
                    .Select(MapToDtoExpression())
                    .ToListAsync();
        }

        /// <summary>
        /// Sends a query to the database to retrieve a staff object based on a specific ID and maps it to the DTO 
        /// object using the MapToDtoExpression() method.
        /// </summary>
        /// <param name="id">The ID of the staff to retrieve.</param>
        /// <returns>Returns an EditStaffDTO object if found, otherwise null.</returns>
        public async Task<EditStaffDTO?> GetStaffByIdAsync(Guid id)
        {
            return await _context.Staffs
                    .AsNoTracking()
                    .Where(s => s.StaffID == id)
                    .Select(MapToDtoExpression())
                    .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new staff object in the database based on the EditStaffDTO object. 
        /// Before creating the staff, it checks if the provided DepartmentID exists in the Departments table. 
        /// If it does not exist, an ArgumentException is thrown. 
        /// If the department exists, a new Staff entity is created and added to the database, and then saved. 
        /// Finally, the created staff is mapped to an EditStaffDTO and returned.
        /// </summary>
        /// <param name="dto">DTO object containing staff information.</param>
        /// <returns>Returns the created EditStaffDTO object.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided DepartmentID does not exist.</exception>
        public async Task<EditStaffDTO?> PostStaffAsync(EditStaffDTO dto)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.DepartmentID == dto.DepartmentID);

            if (!departmentExists)
                throw new ArgumentException("DepartmentID does not exist.");

            var staff = new Staff
            {
                StaffID = dto.StaffID == Guid.Empty ? Guid.NewGuid() : dto.StaffID,
                StaffName = dto.StaffName,
                Initials = dto.Initials,
                Role = dto.Role,
                DepartmentID = dto.DepartmentID,
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            var createdStaff = await GetStaffByIdAsync(staff.StaffID);
            return createdStaff;
        }

        /// <summary>
        /// Updates staff object in the database, based on the provided ID and EditStaffDTO object.
        /// </summary>
        /// <param name="id">The ID of the staff to update.</param>
        /// <param name="dto">DTO object containing updated staff information.</param>
        /// <returns>Returns true if the update is successful, otherwise false.</returns>
        public async Task<bool> PutStaffAsync(Guid id, EditStaffDTO dto)
        {
            var existingStaff = await _context.Staffs
                .FirstOrDefaultAsync(s => s.StaffID == id);

            if (existingStaff == null)
            {
                return false;
            }

            existingStaff.StaffName = dto.StaffName;
            existingStaff.Initials = dto.Initials;
            existingStaff.Role = dto.Role;
            existingStaff.DepartmentID = dto.DepartmentID;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deleted a staff object from the database, based on the given ID.
        /// </summary>
        /// <param name="id">The ID of the staff to delete.</param>
        /// <returns>Returns true if the deletion is successful, otherwise false.</returns>
        public async Task<bool> DeleteStaffAsync(Guid id)
        {
            var staff = await _context.Staffs
                .FirstOrDefaultAsync(s => s.StaffID == id);

            if (staff == null)
            {
                return false;
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Creates an expression that maps a Staff entity to an EditStaffDTO for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of Staff entities to EditStaffDTO objects.</remarks>
        /// <returns>An expression that projects a Staff object into an EditStaffDTO instance.</returns>
        private static Expression<Func<Staff, EditStaffDTO>> MapToDtoExpression()
        {
            return staff => new EditStaffDTO
            {
                StaffID = staff.StaffID,
                StaffName = staff.StaffName,
                Initials = staff.Initials,
                Role = staff.Role,
                DepartmentID = staff.DepartmentID
            };
        }
    }
}
