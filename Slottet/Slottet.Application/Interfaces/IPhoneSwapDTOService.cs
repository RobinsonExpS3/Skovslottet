using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IPhoneSwapDTOService
    {
        Task<IEnumerable<PhoneSwapDTO>> GetAllAsync();
        Task<PhoneSwapDTO?> GetByIdAsync(Guid id);
        Task<PhoneSwapDTO> CreateAsync(PhoneSwapDTO dto);
        Task<bool> UpdateAsync(Guid id, PhoneSwapDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
