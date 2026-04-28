using Slottet.Shared;
using Slottet.Domain.Entities;

namespace Slottet.Application.Interfaces
{
    public interface IShiftBoardDTOService
    {
        Task<ShiftBoardDTO?>  GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ShiftBoardDTO?>  GetLatestAsync(CancellationToken ct = default);
        Task<IEnumerable<ShiftBoard>> GetAllShiftBoardsAsync(CancellationToken ct = default);
        Task<ShiftBoard?> GetShiftBoardByIdAsync(Guid id, CancellationToken ct = default);
        Task<ShiftBoard> CreateShiftBoardAsync(ShiftBoard shiftBoard, CancellationToken ct = default);
        Task<bool> UpdateShiftBoardAsync(Guid id, ShiftBoard shiftBoard, CancellationToken ct = default);
        Task<bool> DeleteShiftBoardAsync(Guid id, CancellationToken ct = default);
    }
}
