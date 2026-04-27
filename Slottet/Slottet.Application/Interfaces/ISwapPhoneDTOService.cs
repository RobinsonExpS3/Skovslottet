using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISwapPhoneDTOService
    {
        Task<IEnumerable<SwapPhoneDTO>> GetAllAsync();
        //Task<SwapPhoneDTO?> GetByIdAsync(Guid phoneId);
        Task<bool> UpdateAsync(SwapPhoneDTO dto);

    }
}
