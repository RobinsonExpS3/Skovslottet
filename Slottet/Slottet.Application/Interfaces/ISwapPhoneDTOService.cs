using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISwapPhoneDTOService
    {
        Task<IEnumerable<SwapPhoneDTO>> GetAllAsync();
        //Task<SwapPhoneDTO?> GetByIdAsync(Guid phoneId);
        Task<bool> UpdateAsync(Guid phoneID, Guid staffID, SwapPhoneDTO dto);

    }
}
