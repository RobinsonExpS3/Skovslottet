using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.Infrastructure.Services
{
    public class GroceryDayDTOService : IGroceryDayDTOService
    {
        private readonly SlottetDBContext _context;

        public GroceryDayDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResidentLookupDTO>> GetAllAsync()
        {
            return await _context.GroceryDays
                .AsNoTracking()
                .OrderBy(g => g.GroceryDayName)
                .Select(g => new ResidentLookupDTO
                {
                    ID = g.GroceryDayID,
                    Name = g.GroceryDayName
                })
                .ToListAsync();
        }
    }
}
