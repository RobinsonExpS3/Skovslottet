using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialResponsibilityController : Controller
    {
        private readonly ISpecialResponsibilityDTOService _specialResponsibilityService;

        public SpecialResponsibilityController(ISpecialResponsibilityDTOService specialResponsibilityService)
        {
            _specialResponsibilityService = specialResponsibilityService;
        }

        /// <summary>
        /// Gets all special responsibilities as DTO objects.
        /// </summary>
        /// <returns>Returns all special responsibilities.</returns>
        [HttpGet("SpecialResponsibilities")]
        public async Task<ActionResult<IEnumerable<SpecialResponsibilityEntryDto>>> GetAllSpecialResponsibilitiesAsync()
        {
            var specialResponsibilities = await _specialResponsibilityService.GetAllSpecialResponsibilitiesAsync();

            return Ok(specialResponsibilities);
        }

        /// <summary>
        /// Gets a special responsibility by ID.
        /// </summary>
        /// <param name="id">The ID of the special responsibility to retrieve.</param>
        /// <returns>Returns the special responsibility if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialResponsibilityEntryDto>> GetSpecialResponsibilityByIdAsync(Guid id)
        {
            var specialResponsibility = await _specialResponsibilityService.GetSpecialResponsibilityByIdAsync(id);

            if (specialResponsibility == null)
            {
                return NotFound();
            }

            return Ok(specialResponsibility);
        }

        /// <summary>
        /// Creates a new special responsibility.
        /// </summary>
        /// <param name="dto">DTO object containing special responsibility information.</param>
        /// <returns>Returns the created special responsibility.</returns>
        [HttpPost]
        public async Task<ActionResult<SpecialResponsibilityEntryDto>> PostSpecialResponsibilityAsync([FromBody] SpecialResponsibilityEntryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest();
            }

            var specialResponsibility = await _specialResponsibilityService.PostSpecialResponsibilityAsync(dto);
            if (specialResponsibility == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetSpecialResponsibilityByIdAsync), new { id = specialResponsibility.SpecialResponsibilityID }, specialResponsibility);
        }

        /// <summary>
        /// Updates a special responsibility by ID.
        /// </summary>
        /// <param name="id">The ID of the special responsibility to update.</param>
        /// <param name="dto">DTO object containing updated special responsibility information.</param>
        /// <returns>Returns NoContent if the update succeeds, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> PutSpecialResponsibilityAsync(Guid id, [FromBody] SpecialResponsibilityEntryDto dto)
        {
            if (dto == null || id != dto.SpecialResponsibilityID || string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest();
            }

            var updated = await _specialResponsibilityService.PutSpecialResponsibilityAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a special responsibility by ID.
        /// </summary>
        /// <param name="id">The ID of the special responsibility to delete.</param>
        /// <returns>Returns NoContent if the deletion succeeds, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSpecialResponsibilityAsync(Guid id)
        {
            var deleted = await _specialResponsibilityService.DeleteSpecialResponsibilityAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
