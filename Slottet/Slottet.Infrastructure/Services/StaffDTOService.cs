using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slottet.Infrastructure.Services {
    public class StaffDTOService : IStaffDTOService {
        private readonly SlottetDBContext _context;

        public StaffDTOService(SlottetDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<EditStaffDTO>> GetAllAsync() {
            return await _context.Staffs
                    .AsNoTracking()
                    .OrderBy(s => s.StaffID)
                    .Select(MapToDtoExpression())
                    .ToListAsync();
        }

        public async Task<EditStaffDTO?> GetByIdAsync(Guid id) {
            return await _context.Staffs
                    .AsNoTracking()
                    .Where(s => s.StaffID == id)
                    .Select(MapToDtoExpression())
                    .FirstOrDefaultAsync();
        }

        public async Task<EditStaffDTO> CreateAsync(EditStaffDTO dto) {
            var staff = new Staff {
                StaffID = dto.StaffID,
                StaffName = dto.StaffName,
                Initials = dto.Initials,
                Role = dto.Role,
                DepartmentID = dto.DepartmentID,
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            return MapToDTO(staff);
        }
        
        public async Task<bool> UpdateAsync(Guid id, EditStaffDTO dto) {
            var existingStaff = await _context.Staffs
                .FirstOrDefaultAsync(s => s.StaffID == id);

            if(existingStaff != null) {
                return false;
            }

            existingStaff.StaffID = dto.StaffID;
            existingStaff.StaffName = dto.StaffName;
            existingStaff.Initials = dto.Initials;
            existingStaff.Role = dto.Role;
            existingStaff.DepartmentID = dto.DepartmentID;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id) {
            var staff = await _context.Staffs
                .FirstOrDefaultAsync(s => s.StaffID == id);

            if(staff == null) {
                return false;
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        private static System.Linq.Expressions.Expression<Func<Staff, EditStaffDTO>> MapToDtoExpression() {
            return staff => new EditStaffDTO {
                StaffID = staff.StaffID,
                StaffName = staff.StaffName,
                Initials = staff.Initials,
                Role = staff.Role,
                DepartmentID = staff.DepartmentID
            };
        }

        private static EditStaffDTO MapToDTO(Staff staff) {
            return new EditStaffDTO {
                StaffID = staff.StaffID,
                StaffName = staff.StaffName,
                Initials = staff.Initials,
                Role = staff.Role,
                DepartmentID = staff.DepartmentID
            };
        }
    }
}
