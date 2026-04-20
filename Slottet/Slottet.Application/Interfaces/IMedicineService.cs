using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IMedicineService
    {
        Task<IEnumerable<MedicineAPI_DTO>> GetAllAsync();
        Task<MedicineAPI_DTO> GetByIdAsync(Guid id);
        Task<MedicineAPI_DTO> CreateAsync(MedicineAPI_DTO dto);
        Task UpdateAsync(Guid id, MedicineAPI_DTO dto);
        Task DeleteAsync(Guid id);
    }
}
