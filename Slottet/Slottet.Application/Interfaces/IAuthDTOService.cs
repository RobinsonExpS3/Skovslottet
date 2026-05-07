using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IAuthDTOService
    {
        Task<CreateUserForStaffDTO?> GetStaffUserAsync(Guid userId);
        Task<CreateUserForStaffDTO?> PostStaffUserAsync(CreateUserForStaffDTO dto);
        Task<bool> PutStaffUserAsync(Guid id, CreateUserForStaffDTO dto);
        Task<bool> DeleteStaffUserAsync(Guid id);
    }
}
