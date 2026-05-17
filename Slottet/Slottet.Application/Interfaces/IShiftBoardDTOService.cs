using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IShiftBoardDTOService
    {
        Task<ShiftBoardDTO?> GetCurrentShiftBoardAsync(CancellationToken ct = default);
        Task<ShiftBoardDTO?> GetShiftBoardByDateAndShiftAsync(DateOnly date, string shiftType, CancellationToken ct = default);
        Task<ShiftBoardDTO?> GetShiftBoardDtoByIdAsync(Guid id, CancellationToken ct = default);
        Task<ShiftBoardDTO?> GetPreviousShiftBoardAsync(Guid currentId, CancellationToken ct = default);
        Task<ShiftBoardDTO?> GetNextShiftBoardAsync(Guid currentId, CancellationToken ct = default);
        Task<ShiftBoardDTO?> GetOrCreateNextShiftBoardAsync(Guid currentId, CancellationToken ct = default);
        Task<IEnumerable<ShiftBoardEntryDTO>> GetAllShiftBoardsAsync(CancellationToken ct = default);
        Task<ShiftBoardEntryDTO?> GetShiftBoardByIdAsync(Guid id, CancellationToken ct = default);
        Task<ShiftBoardEntryDTO?> PostShiftBoardAsync(ShiftBoardEntryDTO dto, CancellationToken ct = default);
        Task<bool> PutShiftBoardAsync(Guid id, ShiftBoardEntryDTO dto, CancellationToken ct = default);
        Task<bool> UpdateResidentCardAsync(ResidentCardDto dto, CancellationToken ct = default);
        Task<bool> UpdatePhoneAssignmentAsync(SwapPhoneDTO dto, CancellationToken ct = default);
        Task<bool> UpdateSpecialResponsibilityAssignmentAsync(SpecialResponsibilityAssignmentDto dto, CancellationToken ct = default);
    }
}
