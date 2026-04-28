using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISpecialResponsibilityDTOService
    {
        Task<IEnumerable<SpecialResponsibilityEntryDto>> GetAllAsync();
        Task<SpecialResponsibilityEntryDto?> GetByIdAsync(Guid id);
        Task<SpecialResponsibilityEntryDto> CreateAsync(SpecialResponsibilityEntryDto dto);
        Task<bool> UpdateAsync(Guid id, SpecialResponsibilityEntryDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
