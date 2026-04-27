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
    public class SpecialResponsibilityController : Controller {
        private readonly SlottetDBContext _context;

        public SpecialResponsibilityController(SlottetDBContext context) {
            _context = context;
        }

        //Get: special responsibility
        [HttpGet("SpecialResponsibilities")]
        public async Task<ActionResult<IEnumerable<SpecialResponsibility>>> GetAllAsync() {
            var specialResponsibility = await _context.SpecialResponsibilities
                .AsNoTracking()
                .ToListAsync();

            return Ok(specialResponsibility);
        }

        //Get: special responsibility by id
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialResponsibility>> GetByIdAsync(Guid id) {
            var specialResponsibility = await _context.SpecialResponsibilities
                .AsNoTracking()
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);

            if (specialResponsibility == null) {
                return NotFound();
            }

            return Ok(specialResponsibility);
        }

        //Post: special responsibility
        [HttpPost]
        public async Task<ActionResult<SpecialResponsibility>> CreateAsync([FromBody] SpecialResponsibility specialResponsibility) {
            if (specialResponsibility == null || string.IsNullOrWhiteSpace(specialResponsibility.TaskName) || specialResponsibility.ShiftBoardID == Guid.Empty) {
                return BadRequest();
            }

            if(specialResponsibility.SpecialResponsibilityID == Guid.Empty) {
                specialResponsibility.SpecialResponsibilityID = Guid.NewGuid();
            }

            _context.SpecialResponsibilities.Add(specialResponsibility);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = specialResponsibility.SpecialResponsibilityID }, specialResponsibility);
        }

        //Put: special responsibility by id
        [HttpPut("{id}")]
        public async Task<ActionResult<SpecialResponsibility>> UpdateAsync(Guid id, [FromBody] SpecialResponsibility specialResponsibility) {
            if (specialResponsibility == null || id != specialResponsibility.SpecialResponsibilityID || string.IsNullOrWhiteSpace(specialResponsibility.TaskName) || specialResponsibility.ShiftBoardID == Guid.Empty) {
                return BadRequest();
            }

            var existingSpecialResponsibility = await _context.SpecialResponsibilities
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);

            if (existingSpecialResponsibility == null) {
                return NotFound();
            }

            existingSpecialResponsibility.TaskName = specialResponsibility.TaskName;
            existingSpecialResponsibility.ShiftBoardID = specialResponsibility.ShiftBoardID;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Delete: special responsibility by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id) {
            var existingSpecialResponsibility = await _context.SpecialResponsibilities
                .FirstOrDefaultAsync(sr => sr.SpecialResponsibilityID == id);

            if (existingSpecialResponsibility == null) {
                return NotFound();
            }

            _context.SpecialResponsibilities.Remove(existingSpecialResponsibility);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
