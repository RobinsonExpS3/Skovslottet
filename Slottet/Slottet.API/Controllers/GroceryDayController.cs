using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroceryDayController : Controller {
        private readonly SlottetDBContext _context;

        public GroceryDayController(SlottetDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetAllAsync() {
            var methods = await _context.GroceryDays
                .AsNoTracking()
                .OrderBy(g => g.GroceryDayName)
                .Select(g => new ResidentLookupDTO {
                    ID = g.GroceryDayID,
                    Name = g.GroceryDayName
                })
                .ToListAsync();

            return Ok(methods);
        }
    }
}
