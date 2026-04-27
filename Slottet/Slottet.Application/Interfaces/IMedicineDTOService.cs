using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IMedicineDTOService
    {
        Task<IEnumerable<MedicineAPI_DTO>> GetAllAsync();
        Task<MedicineAPI_DTO?> GetByIdAsync(Guid id);
        Task<MedicineAPI_DTO> CreateAsync(MedicineAPI_DTO dto);
        Task<bool> UpdateAsync(Guid id, MedicineAPI_DTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
