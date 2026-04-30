using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISwapPhoneDTOService
    {
        Task<IEnumerable<SwapPhoneRecDTO>> GetAllAsync();
        Task<bool> UpdateAsync(SwapPhoneRecDTO dto);

    }
}
