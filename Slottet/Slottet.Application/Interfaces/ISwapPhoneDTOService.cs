using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISwapPhoneDTOService
    {
        Task<IEnumerable<SwapPhoneRecDTO>> GetAllAsync();
        //Task<SwapPhoneRecDTO?> GetByIdAsync(Guid phoneId);
        Task<bool> UpdateAsync(SwapPhoneRecDTO dto);

    }
}
