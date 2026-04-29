using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftboardController : ControllerBase
    {
        private readonly IShiftBoardDTOService _shiftBoardService;

        public ShiftboardController(IShiftBoardDTOService shiftBoardService)
        {
            _shiftBoardService = shiftBoardService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<ShiftBoardDTO>> GetCurrent(CancellationToken ct)
        {
            var dto = await _shiftBoardService.GetLatestAsync(ct);

            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpGet("{id:guid}/dto")]
        public async Task<ActionResult<ShiftBoardDTO>> GetDto(Guid id, CancellationToken ct)
        {
            var dto = await _shiftBoardService.GetByIdAsync(id, ct);

            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftBoard>>> GetAll(CancellationToken ct)
        {
            var shiftboards = await _shiftBoardService.GetAllShiftBoardsAsync(ct);

            return Ok(shiftboards);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ShiftBoard>> GetById(Guid id, CancellationToken ct)
        {
            var shiftboard = await _shiftBoardService.GetShiftBoardByIdAsync(id, ct);

            if (shiftboard == null)
            {
                return NotFound();
            }

            return Ok(shiftboard);
        }

        [HttpPost]
        public async Task<ActionResult<ShiftBoard>> Create([FromBody] ShiftBoard shiftboard, CancellationToken ct)
        {
            if (shiftboard == null)
            {
                return BadRequest();
            }

            var createdShiftBoard = await _shiftBoardService.CreateShiftBoardAsync(shiftboard, ct);

            return CreatedAtAction(nameof(GetById), new { id = createdShiftBoard.ShiftBoardID }, createdShiftBoard);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ShiftBoard shiftboard, CancellationToken ct)
        {
            if (id != shiftboard.ShiftBoardID)
            {
                return BadRequest("Id i URL matcher ikke objektets id.");
            }

            var updated = await _shiftBoardService.UpdateShiftBoardAsync(id, shiftboard, ct);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var deleted = await _shiftBoardService.DeleteShiftBoardAsync(id, ct);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
