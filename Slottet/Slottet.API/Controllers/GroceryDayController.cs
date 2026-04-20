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
        public async Task<ActionResult<IEnumerable<GroceryDayDTO>>> GetAll() {
            var methods = await _context.GroceryDays
                .AsNoTracking()
                .Select(g => new GroceryDayDTO {
                    GroceryDayID = g.GroceryDayID,
                    GroceryDayName = g.GroceryDayName
                })
                .ToListAsync();

            return Ok(methods);
        }
    }
}
