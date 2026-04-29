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
            var specialresponsibilities = await _context.SpecialResponsibilities
                .AsNoTracking()
                .OrderBy(sr => sr.TaskName)
                .Select(MapToDtoExpression())
                .ToListAsync();

            var staffs = await GetStaffLookupAsync();

            foreach (var specialResponsibility in specialresponsibilities)
            {
                var staff = staffs.FirstOrDefault(s => s.StaffName == specialResponsibility.StaffName);
                if (staff != null)
                {
                    specialResponsibility.StaffName = staff.StaffName;
                    specialResponsibility.StaffInitials = staff.Initials;
                }
            }

            return specialresponsibilities;
        }

        public async Task<SpecialResponsibilityEntryDto?> GetByIdAsync(Guid id)
        {

            var specialResponsibility = await _context.SpecialResponsibilities
                .AsNoTracking()
                .Where(sr => sr.SpecialResponsibilityID == id)
                .Select(MapToDtoExpression())
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);

            if (specialResponsibility != null)
            {
                var staff = await _context.Staffs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.StaffName == specialResponsibility.StaffName);
                if (staff != null)
                {
                    specialResponsibility.StaffName = staff.StaffName;
                    specialResponsibility.StaffInitials = staff.Initials;
                }
            }

            return specialResponsibility;
        }

        public async Task<SpecialResponsibilityEntryDto> CreateAsync(SpecialResponsibilityEntryDto dto)
        {
            if (specialResponsibility.SpecialResponsibilityID == Guid.Empty)
            {
                specialResponsibility.SpecialResponsibilityID = Guid.NewGuid();
            }

            _context.SpecialResponsibilities.Add(specialResponsibility);
            await _context.SaveChangesAsync();

            var specialResponsibility = new SpecialResponsibility
            {
                SpecialResponsibilityID = dto.SpecialResponsibilityID == Guid.Empty ?
                    Guid.NewGuid() : dto.SpecialResponsibilityID,
                TaskName = dto.Description,

            };

            return specialResponsibility;
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

            _context.SpecialResponsibilities.Remove(specialResponsibility);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<List<StaffLookupDto>> GetStaffLookupAsync()
        {
            return await _context.Staffs
                .AsNoTracking()
                .Select(s => new StaffLookupDto
                {
                    ID = s.StaffID,
                    StaffName = s.StaffName,
                    Initials = s.Initials,
                    Role = s.Role
                })
                .ToListAsync();
        }

        private static Expression<Func<SpecialResponsibility, SpecialResponsibilityEntryDto>> MapToDtoExpression()
        {
            return specialResponsibility => new SpecialResponsibilityEntryDto
            {
                SpecialResponsibilityID = specialResponsibility.SpecialResponsibilityID,
                Description = specialResponsibility.TaskName,
                StaffName = specialResponsibility.Staff != null ? $"{specialResponsibility.Staff.FirstName} {specialResponsibility.Staff.LastName}" : "Unassigned"
            };
        }
    }
}
