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
                .ToListAsync();
        }

        public async Task<SpecialResponsibility?> GetByIdAsync(Guid id)
        {
            return await _context.SpecialResponsibilities
                .AsNoTracking()
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);
        }

        public async Task<SpecialResponsibility> CreateAsync(SpecialResponsibilityEntryDto dto)
        {
            if (specialResponsibility.SpecialResponsibilityID == Guid.Empty)
            {
                specialResponsibility.SpecialResponsibilityID = Guid.NewGuid();
            }

            _context.SpecialResponsibilities.Add(specialResponsibility);
            await _context.SaveChangesAsync();

            return specialResponsibility;
        }

        public async Task<bool> UpdateAsync(Guid id, SpecialResponsibility specialResponsibility)
        {
            var existingSpecialResponsibility = await _context.SpecialResponsibilities
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);

            if (existingSpecialResponsibility == null)
            {
                return false;
            }

            existingSpecialResponsibility.TaskName = specialResponsibility.TaskName;

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
    }
}
