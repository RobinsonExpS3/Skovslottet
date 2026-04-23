using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces {
    public interface IResidentDTOService {
        Task<IEnumerable<EditResidentDTO>> GetAllAsync();
        Task<EditResidentDTO?> GetByIdAsync(Guid id);
        Task<EditResidentDTO> CreateAsync(EditResidentDTO dto);
        Task<bool> UpdateAsync(Guid id, EditResidentDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
