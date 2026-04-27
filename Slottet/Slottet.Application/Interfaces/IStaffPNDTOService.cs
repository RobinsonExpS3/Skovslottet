using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IStaffPNDTOService
    {
        Task<IEnumerable<StaffPNDTO>> GetAllAsync();
        Task<bool> UpdateAsync(StaffPNDTO dto);

    }
}
