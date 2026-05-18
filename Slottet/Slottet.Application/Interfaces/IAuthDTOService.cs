using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IAuthDtoService
    {
        Task<CreateUserForStaffDto?> GetStaffUserAsync(Guid userId);
        Task<CreateUserForStaffDto?> PostStaffUserAsync(CreateUserForStaffDto dto);
        Task<bool> PutStaffUserAsync(Guid id, CreateUserForStaffDto dto);
        Task<bool> DeleteStaffUserAsync(Guid id);
    }
}
