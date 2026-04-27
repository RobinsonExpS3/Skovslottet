using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IShiftBoardDTOService
    {
        Task<ShiftBoardDTO?>  GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ShiftBoardDTO?>  GetLatestAsync(CancellationToken ct = default);
    }
}
