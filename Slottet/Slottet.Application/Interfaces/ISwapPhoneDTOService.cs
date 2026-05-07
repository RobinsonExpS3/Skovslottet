using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISwapPhoneDTOService
    {
        Task<IEnumerable<SwapPhoneDTO>> GetAllSwapPhonesAsync();
        Task<bool> PostSwapPhoneAsync(SwapPhoneDTO dto);

    }
}
