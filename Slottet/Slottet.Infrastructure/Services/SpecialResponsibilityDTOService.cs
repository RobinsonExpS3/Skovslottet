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

        public async Task<IEnumerable<SpecialResponsibilityEntryDto>> GetAllAsync()
        {
            return await _context.SpecialResponsibilities
                .AsNoTracking()
                .OrderBy(sr => sr.TaskName)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }


        public async Task<SpecialResponsibilityEntryDto?> GetByIdAsync(Guid id)
        {
            return await _context.SpecialResponsibilities
                .AsNoTracking()
                .Where(sr => sr.SpecialResponsibilityID == id)
                .Select(MapToDtoExpression())
                .FirstOrDefaultAsync();
        }

        public async Task<SpecialResponsibilityEntryDto> CreateAsync(SpecialResponsibilityEntryDto dto)
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

            var createdSpecialResponsibility = await GetByIdAsync(specialResponsibility.SpecialResponsibilityID);
            return createdSpecialResponsibility ?? MapToDTO(specialResponsibility);
        }

        public async Task<bool> UpdateAsync(Guid id, SpecialResponsibilityEntryDto dto)
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

        public async Task<bool> DeleteAsync(Guid id)
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

        private static SpecialResponsibilityEntryDto MapToDTO(SpecialResponsibility specialResponsibility)
        {
            return new SpecialResponsibilityEntryDto
            {
                SpecialResponsibilityID = specialResponsibility.SpecialResponsibilityID,
                Description = specialResponsibility.TaskName,
                StaffName = "Unassigned"
            };
        }
    }
}
