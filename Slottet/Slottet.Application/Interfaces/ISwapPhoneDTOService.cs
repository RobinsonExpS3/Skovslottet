using System;
using System.Collections.Generic;
using System.Text;
using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISwapPhoneDTOService
    {
        Task<IEnumerable<SwapPhoneDTO>> GetAllAsync();
        Task<SwapPhoneDTO?> GetByIdAsync(Guid phoneId);
        Task<bool> UpdateAsync(Guid id, SwapPhoneDTO dto);
        
    }
}
