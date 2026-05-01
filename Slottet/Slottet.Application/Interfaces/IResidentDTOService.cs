using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces {
    public interface IResidentDTOService {
        Task<IEnumerable<EditResidentDTO>> GetAllResidentsAsync();
        Task<EditResidentDTO?> GetResidentByIdAsync(Guid id);
        Task<EditResidentDTO?> PostResidentAsync(EditResidentDTO dto);
        Task<bool> PutResidentAsync(Guid id, EditResidentDTO dto);
        Task<bool> DeleteResidentAsync(Guid id);

        Task<List<ResidentLookupDTO>> GetResidentGroceryDaysAsync();
        Task<List<ResidentLookupDTO>> GetResidentPaymentMethodsAsync();
    }
}
