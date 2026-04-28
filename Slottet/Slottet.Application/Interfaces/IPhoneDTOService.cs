using Slottet.Domain.Entities;

namespace Slottet.Application.Interfaces
{
    public interface IPhoneDTOService
    {
        Task<IEnumerable<Phone>> GetAllAsync();
        Task<Phone?> GetByIdAsync(Guid id);
        Task<Phone> CreateAsync(Phone phone);
        Task<bool> UpdateAsync(Guid id, Phone phone);
        Task<bool> DeleteAsync(Guid id);
    }
}
