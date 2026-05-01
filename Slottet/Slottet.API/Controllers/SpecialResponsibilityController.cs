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

        //Get: special responsibility
        [HttpGet("SpecialResponsibilities")]
        public async Task<ActionResult<IEnumerable<SpecialResponsibilityEntryDto>>> GetAllAsync()
        {
            var specialResponsibilities = await _specialResponsibilityService.GetAllAsync();

            return Ok(specialResponsibilities);
        }

        //Get: special responsibility by id
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialResponsibilityEntryDto>> GetByIdAsync(Guid id)
        {
            var specialResponsibility = await _specialResponsibilityService.GetByIdAsync(id);

            if (specialResponsibility == null)
            {
                return NotFound();
            }

            return Ok(specialResponsibility);
        }

        //Post: special responsibility
        [HttpPost]
        public async Task<ActionResult<SpecialResponsibilityEntryDto>> CreateAsync([FromBody] SpecialResponsibilityEntryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest();
            }

            var specialResponsibility = await _specialResponsibilityService.CreateAsync(dto);

            return CreatedAtAction("GetById", new { id = specialResponsibility.SpecialResponsibilityID }, specialResponsibility);
        }

        //Put: special responsibility by id
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] SpecialResponsibilityEntryDto dto)
        {
            if (dto == null || id != dto.SpecialResponsibilityID || string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest();
            }

            var updated = await _specialResponsibilityService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        //Delete: special responsibility by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _specialResponsibilityService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
