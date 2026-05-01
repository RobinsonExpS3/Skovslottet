using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;
using System.Linq.Expressions;

namespace Slottet.Infrastructure.Services
{
    public class GroceryDayDTOService : IGroceryDayDTOService
    {
        private readonly SlottetDBContext _context;

        public GroceryDayDTOService(SlottetDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sends a query to the database to retrieve all grocery day objects and maps them to lookup DTO objects.
        /// </summary>
        /// <returns>Returns a list of ResidentLookupDTO objects.</returns>
        public async Task<IEnumerable<ResidentLookupDTO>> GetAllGroceryDaysAsync()
        {
            return await _context.GroceryDays
                .AsNoTracking()
                .OrderBy(g => g.GroceryDayName)
                .Select(MapToDtoExpression())
                .ToListAsync();
        }

        /// <summary>
        /// Creates an expression that maps a GroceryDay entity to a ResidentLookupDTO for use in LINQ queries.
        /// </summary>
        /// <remarks>This expression can be used with LINQ providers such as Entity Framework to perform
        /// efficient server-side projection of GroceryDay entities to ResidentLookupDTO objects.</remarks>
        /// <returns>An expression that projects a GroceryDay object into a ResidentLookupDTO instance.</returns>
        private static Expression<Func<GroceryDay, ResidentLookupDTO>> MapToDtoExpression()
        {
            return groceryDay => new ResidentLookupDTO
            {
                ID = groceryDay.GroceryDayID,
                Name = groceryDay.GroceryDayName
            };
        }
    }
}
