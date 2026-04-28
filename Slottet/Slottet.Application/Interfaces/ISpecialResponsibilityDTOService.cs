using Slottet.Domain.Entities;

namespace Slottet.Application.Interfaces
{
    public interface ISpecialResponsibilityDTOService
    {
        Task<IEnumerable<SpecialResponsibility>> GetAllAsync();
        Task<SpecialResponsibility?> GetByIdAsync(Guid id);
        Task<SpecialResponsibility> CreateAsync(SpecialResponsibility specialResponsibility);
        Task<bool> UpdateAsync(Guid id, SpecialResponsibility specialResponsibility);
        Task<bool> DeleteAsync(Guid id);
    }
}
