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
    public class ShiftboardController : ControllerBase
    {
        private readonly SlottetDBContext      _context;
        private readonly IShiftBoardDTOService _shiftBoardService;

        public ShiftboardController(SlottetDBContext context, IShiftBoardDTOService shiftBoardService)
        {
            _context           = context;
            _shiftBoardService = shiftBoardService;
        }

        // ── DTO endpoints ─────────────────────────────────────────────────

        /// <summary>Returns the most recently started ShiftBoard as a fully assembled DTO.</summary>
        [HttpGet("current")]
        public async Task<ActionResult<ShiftBoardDTO>> GetCurrent(CancellationToken ct)
        {
            var dto = await _shiftBoardService.GetLatestAsync(ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        /// <summary>Returns a specific ShiftBoard as a fully assembled DTO.</summary>
        [HttpGet("{id:guid}/dto")]
        public async Task<ActionResult<ShiftBoardDTO>> GetDto(Guid id, CancellationToken ct)
        {
            var dto = await _shiftBoardService.GetByIdAsync(id, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        // ── Raw CRUD endpoints ────────────────────────────────────────────

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftBoard>>> GetAll(CancellationToken ct)
        {
            var shiftboards = await _context.ShiftBoards
                .AsNoTracking()
                .OrderBy(s => s.StartDateTime)
                .ToListAsync(ct);

            return Ok(shiftboards);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ShiftBoard>> GetById(Guid id, CancellationToken ct)
        {
            var shiftboard = await _context.ShiftBoards
                .AsNoTracking()
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);

            if (shiftboard is null)
                return NotFound();

            return Ok(shiftboard);
        }

        [HttpPost]
        public async Task<ActionResult<ShiftBoard>> Create([FromBody] ShiftBoard shiftboard, CancellationToken ct)
        {
            if (shiftboard == null)
                return BadRequest();

            _context.ShiftBoards.Add(shiftboard);
            await _context.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = shiftboard.ShiftBoardID }, shiftboard);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ShiftBoard shiftboard, CancellationToken ct)
        {
            if (id != shiftboard.ShiftBoardID)
                return BadRequest("Id i URL matcher ikke objektets id.");

            try
            {
                _context.Entry(shiftboard).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ShiftBoards.AnyAsync(sb => sb.ShiftBoardID == id, ct))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var shiftboard = await _context.ShiftBoards
                .FirstOrDefaultAsync(sb => sb.ShiftBoardID == id, ct);

            if (shiftboard == null)
                return NotFound();

            _context.ShiftBoards.Remove(shiftboard);
            await _context.SaveChangesAsync(ct);

            return NoContent();
        }
    }
}
