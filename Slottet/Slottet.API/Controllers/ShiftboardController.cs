using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
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

        /// <summary>
        /// Gets the latest shift board as a DTO object.
        /// </summary>
        /// <param name="ct">Cancellation token used to cancel the request.</param>
        /// <returns>Returns the latest shift board if found, otherwise NotFound.</returns>
        [HttpGet("current")]
        public async Task<ActionResult<ShiftBoardDTO>> GetCurrentShiftBoardAsync(CancellationToken ct)
        {
            var dto = await _shiftBoardService.GetCurrentShiftBoardAsync(ct);

            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        /// <summary>
        /// Gets a shift board DTO by ID.
        /// </summary>
        /// <param name="id">The ID of the shift board to retrieve.</param>
        /// <param name="ct">Cancellation token used to cancel the request.</param>
        /// <returns>Returns the shift board DTO if found, otherwise NotFound.</returns>
        [HttpGet("{id:guid}/dto")]
        public async Task<ActionResult<ShiftBoardDTO>> GetShiftBoardDtoByIdAsync(Guid id, CancellationToken ct)
        {
            var dto = await _shiftBoardService.GetShiftBoardDtoByIdAsync(id, ct);

            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        /// <summary>
        /// Gets all shift boards.
        /// </summary>
        /// <param name="ct">Cancellation token used to cancel the request.</param>
        /// <returns>Returns all shift boards.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftBoardEntryDTO>>> GetAllShiftBoardsAsync(CancellationToken ct)
        {
            var shiftboards = await _shiftBoardService.GetAllShiftBoardsAsync(ct);

            return Ok(shiftboards);
        }

        /// <summary>
        /// Gets a shift board by ID.
        /// </summary>
        /// <param name="id">The ID of the shift board to retrieve.</param>
        /// <param name="ct">Cancellation token used to cancel the request.</param>
        /// <returns>Returns the shift board if found, otherwise NotFound.</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ShiftBoardEntryDTO>> GetShiftBoardByIdAsync(Guid id, CancellationToken ct)
        {
            var shiftboard = await _shiftBoardService.GetShiftBoardByIdAsync(id, ct);

            if (shiftboard == null)
            {
                return NotFound();
            }

            return Ok(shiftboard);
        }

        /// <summary>
        /// Creates a new shift board.
        /// </summary>
        /// <param name="dto">DTO object containing shift board information.</param>
        /// <param name="ct">Cancellation token used to cancel the request.</param>
        /// <returns>Returns the created shift board.</returns>
        [HttpPost]
        public async Task<ActionResult<ShiftBoardEntryDTO>> PostShiftBoardAsync([FromBody] ShiftBoardEntryDTO dto, CancellationToken ct)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var createdShiftBoard = await _shiftBoardService.PostShiftBoardAsync(dto, ct);
            if (createdShiftBoard == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetShiftBoardByIdAsync), new { id = createdShiftBoard.ShiftBoardID }, createdShiftBoard);
        }

        /// <summary>
        /// Updates a shift board by ID.
        /// </summary>
        /// <param name="id">The ID of the shift board to update.</param>
        /// <param name="dto">DTO object containing updated shift board values.</param>
        /// <param name="ct">Cancellation token used to cancel the request.</param>
        /// <returns>Returns NoContent if the update succeeds, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutShiftBoardAsync(Guid id, [FromBody] ShiftBoardEntryDTO dto, CancellationToken ct)
        {
            if (dto == null || id != dto.ShiftBoardID)
            {
                return BadRequest("Id i URL matcher ikke objektets id.");
            }

            var updated = await _shiftBoardService.PutShiftBoardAsync(id, dto, ct);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a shift board by ID.
        /// </summary>
        /// <param name="id">The ID of the shift board to delete.</param>
        /// <param name="ct">Cancellation token used to cancel the request.</param>
        /// <returns>Returns NoContent if the deletion succeeds, otherwise NotFound.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteShiftBoardAsync(Guid id, CancellationToken ct)
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
