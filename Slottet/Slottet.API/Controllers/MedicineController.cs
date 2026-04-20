using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly SlottetDBContext _context;

        public MedicineController(SlottetDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineAPI_DTO>>> GetAll()
        {
            var medicines = await _context.Set<Medicine>()
                .AsNoTracking()
                .OrderBy(m => m.ResidentID)
                .Select(m => new MedicineAPI_DTO
                {
                    MedicineID = m.MedicineID,
                    ResidentID = m.ResidentID,
                    MedicineTime = m.MedicineTime,
                    MedicineGivenTime = m.MedicineGivenTime,
                    MedicineRegisteredTime = m.MedicineRegisteredTime
                })
                .ToListAsync();

            return Ok(medicines);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MedicineAPI_DTO>> GetById(Guid id)
        {
            var medicine = await _context.Set<Medicine>()
                .AsNoTracking()
                .Where(m => m.MedicineID == id)
                .Select(m => new MedicineAPI_DTO
                {
                    MedicineID = m.MedicineID,
                    ResidentID = m.ResidentID,
                    MedicineTime = m.MedicineTime,
                    MedicineGivenTime = m.MedicineGivenTime,
                    MedicineRegisteredTime = m.MedicineRegisteredTime
                })
                .FirstOrDefaultAsync();

            if (medicine == null)
                return NotFound();

            return Ok(medicine);
        }

        [HttpPost]
        public async Task<ActionResult<MedicineAPI_DTO>> Create([FromBody] MedicineAPI_DTO dto)
        {
            if (dto == null || dto.ResidentID == Guid.Empty)
                return BadRequest();

            var medicine = new Medicine
            {
                MedicineID = Guid.NewGuid(),
                ResidentID = dto.ResidentID,
                MedicineTime = dto.MedicineTime,
                MedicineGivenTime = dto.MedicineGivenTime ?? DateTime.Now,
                MedicineRegisteredTime = DateTime.Now
            };

            _context.Set<Medicine>().Add(medicine);
            await _context.SaveChangesAsync();

            var result = new MedicineAPI_DTO
            {
                MedicineID = medicine.MedicineID,
                ResidentID = medicine.ResidentID,
                MedicineTime = medicine.MedicineTime,
                MedicineGivenTime = medicine.MedicineGivenTime,
                MedicineRegisteredTime = medicine.MedicineRegisteredTime
            };

            return CreatedAtAction(nameof(GetById), new { id = medicine.MedicineID }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MedicineAPI_DTO dto)
        {
            if (dto == null || dto.MedicineID != id || dto.ResidentID == Guid.Empty)
                return BadRequest();

            var existingMedicine = await _context.Set<Medicine>()
                .FirstOrDefaultAsync(m => m.MedicineID == id);

            if (existingMedicine == null)
                return NotFound();

            existingMedicine.ResidentID = dto.ResidentID;
            existingMedicine.MedicineTime = dto.MedicineTime;
            existingMedicine.MedicineGivenTime = dto.MedicineGivenTime ?? existingMedicine.MedicineGivenTime;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var medicine = await _context.Set<Medicine>()
                .FirstOrDefaultAsync(m => m.MedicineID == id);

            if (medicine == null)
                return NotFound();

            _context.Set<Medicine>().Remove(medicine);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}