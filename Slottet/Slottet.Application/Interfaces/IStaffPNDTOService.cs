using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IStaffPNDTOService
    {
        Task<IEnumerable<StaffPNDTO>> GetAllStaffPNsAsync();
        Task<bool> PostStaffPNAsync(StaffPNDTO dto);

    }
}
