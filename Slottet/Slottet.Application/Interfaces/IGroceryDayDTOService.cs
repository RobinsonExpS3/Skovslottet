using Slottet.Shared;

namespace Slottet.Application.Interfaces
{
    public interface IGroceryDayDTOService
    {
        Task<IEnumerable<ResidentLookupDTO>> GetAllGroceryDaysAsync();
    }
}
