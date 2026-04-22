using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces {
    public interface IResidentDTOService {
        Task<IEnumerable<ResidentDTO>> GetAllAsync();
        Task<ResidentDTO?> GetByIdAsync(Guid id);
        Task<ResidentDTO> CreateAsync(ResidentDTO dto);
        Task<bool> UpdateAsync(Guid id, ResidentDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
