using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftboardController : ControllerBase
    {
        private readonly SlottetDBContext _context;

        public ShiftboardController(SlottetDBContext context)
        {
            _context = context;
        }

        [HttpGet("ShiftBoards")]
        public async Task<ActionResult<IEnumerable<ShiftBoard>>> GetAllAsync()
        {
            var shiftboards = await _context.Set<ShiftBoard>()
                .AsNoTracking()
                .OrderBy(s => s.StartDateTime)
                .ToListAsync();

            return Ok(shiftboards);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ShiftBoard>> GetByIdAsync(Guid id)
        {
            var shiftboard = await _context.Set<ShiftBoard>()
                .AsNoTracking()
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id);

            if (shiftboard is null)
                return NotFound();

            return Ok(shiftboard);
        }

        [HttpPost]
        public async Task<ActionResult<ShiftBoard>> CreateAsync([FromBody] ShiftBoard shiftboard)
        {
            _context.Set<ShiftBoard>().Add(shiftboard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = shiftboard.ShiftBoardID }, shiftboard);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ShiftBoard shiftboard)
        {
            if (id != shiftboard.ShiftBoardID)
                return BadRequest("Id i URL matcher ikke objektets id.");

            var exists = await _context.Set<ShiftBoard>()
                .AnyAsync(sb => sb.ShiftBoardID == id);

            if (!exists)
                return NotFound();

            _context.Entry(shiftboard).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var shiftboard = await _context.Set<ShiftBoard>()
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id);

            if (shiftboard == null)
                return NotFound();

            _context.Set<ShiftBoard>().Remove(shiftboard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
