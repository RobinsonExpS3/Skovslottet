using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;

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
        public async Task<ActionResult<IEnumerable<SpecialResponsibility>>> GetAllAsync()
        {
            var specialResponsibilities = await _specialResponsibilityService.GetAllAsync();

            return Ok(specialResponsibilities);
        }

        //Get: special responsibility by id
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialResponsibility>> GetByIdAsync(Guid id)
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
        public async Task<ActionResult<SpecialResponsibility>> CreateAsync([FromBody] SpecialResponsibility specialResponsibility) {
            if (specialResponsibility == null || string.IsNullOrWhiteSpace(specialResponsibility.TaskName)) {
                return BadRequest();
            }

            var createdSpecialResponsibility = await _specialResponsibilityService.CreateAsync(specialResponsibility);

            _context.SpecialResponsibilities.Add(specialResponsibility);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = specialResponsibility.SpecialResponsibilityID }, specialResponsibility);
        }

        //Put: special responsibility by id
        [HttpPut("{id}")]
        public async Task<ActionResult<SpecialResponsibility>> UpdateAsync(Guid id, [FromBody] SpecialResponsibility specialResponsibility) {
            if (specialResponsibility == null || id != specialResponsibility.SpecialResponsibilityID || string.IsNullOrWhiteSpace(specialResponsibility.TaskName)) {
                return BadRequest();
            }

            var updated = await _specialResponsibilityService.UpdateAsync(id, specialResponsibility);

            if (!updated)
            {
                return NotFound();
            }

            existingSpecialResponsibility.TaskName = specialResponsibility.TaskName;

            await _context.SaveChangesAsync();

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
