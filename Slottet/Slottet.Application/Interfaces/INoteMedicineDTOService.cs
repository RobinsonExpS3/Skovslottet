using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface INoteMedicineDTOService
    {
        Task<IEnumerable<NoteMedicineDTO>> GetAllAsync();
        Task<NoteMedicineDTO?> GetByIdAsync(Guid id);
        Task<NoteMedicineDTO> CreateAsync(NoteMedicineDTO dto);
        Task<bool> UpdateAsync(Guid id, NoteMedicineDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
