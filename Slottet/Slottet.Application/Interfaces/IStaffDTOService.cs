using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IStaffDTOService
    {
        Task<IEnumerable<EditStaffDTO>> GetAllAsync();
        Task<EditStaffDTO?> GetByIdAsync(Guid id);
        Task<EditStaffDTO> CreateAsync(EditStaffDTO dto);
        Task<bool> UpdateAsync(Guid id, EditStaffDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}