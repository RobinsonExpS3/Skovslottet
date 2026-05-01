using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface ISpecialResponsibilityDTOService
    {
        Task<IEnumerable<SpecialResponsibilityEntryDto>> GetAllSpecialResponsibilitiesAsync();
        Task<SpecialResponsibilityEntryDto?> GetSpecialResponsibilityByIdAsync(Guid id);
        Task<SpecialResponsibilityEntryDto?> PostSpecialResponsibilityAsync(SpecialResponsibilityEntryDto dto);
        Task<bool> PutSpecialResponsibilityAsync(Guid id, SpecialResponsibilityEntryDto dto);
        Task<bool> DeleteSpecialResponsibilityAsync(Guid id);
    }
}
