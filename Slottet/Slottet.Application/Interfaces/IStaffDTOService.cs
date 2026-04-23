using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces {
    public interface IStaffDTOService {
        Task<IEnumerable<StaffDTO>> GetAllAsync();
        Task<StaffDTO?> GetByIdAsync(Guid id);
        Task<StaffDTO> CreateAsync(StaffDTO dto);
        Task<bool> UpdateAsync(Guid id, StaffDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
