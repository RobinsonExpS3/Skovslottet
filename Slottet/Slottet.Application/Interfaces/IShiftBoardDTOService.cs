using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IShiftBoardDTOService
    {
        Task<ShiftBoardDTO?> GetCurrentShiftBoardAsync(CancellationToken ct = default);
        Task<ShiftBoardDTO?> GetShiftBoardDtoByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ShiftBoardEntryDTO>> GetAllShiftBoardsAsync(CancellationToken ct = default);
        Task<ShiftBoardEntryDTO?> GetShiftBoardByIdAsync(Guid id, CancellationToken ct = default);
        Task<ShiftBoardEntryDTO?> PostShiftBoardAsync(ShiftBoardEntryDTO dto, CancellationToken ct = default);
        Task<bool> PutShiftBoardAsync(Guid id, ShiftBoardEntryDTO dto, CancellationToken ct = default);
        Task<bool> DeleteShiftBoardAsync(Guid id, CancellationToken ct = default);
    }
}
