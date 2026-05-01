using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IPhoneDTOService
    {
        Task<IEnumerable<PhoneDTO>> GetAllPhonesAsync();
        Task<PhoneDTO?> GetPhoneByIdAsync(Guid id);
        Task<PhoneDTO?> PostPhoneAsync(PhoneDTO dto);
        Task<bool> PutPhoneAsync(Guid id, PhoneDTO dto);
        Task<bool> DeletePhoneAsync(Guid id);
    }
}
