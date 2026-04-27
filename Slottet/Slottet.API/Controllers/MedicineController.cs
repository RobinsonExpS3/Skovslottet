using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineDTOService _medicineService;

        public MedicineController(IMedicineDTOService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineAPI_DTO>>> GetAll()
        {
            var medicines = await _medicineService.GetAllAsync();
            return Ok(medicines);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MedicineAPI_DTO>> GetById(Guid id)
        {
            var medicine = await _medicineService.GetByIdAsync(id);

            if (medicine == null)
            {
                return NotFound();
            }

            return Ok(medicine);
        }

        [HttpPost]
        public async Task<ActionResult<MedicineAPI_DTO>> Create([FromBody] MedicineAPI_DTO dto)
        {
            if (dto == null || dto.ResidentID == Guid.Empty)
            {
                return BadRequest();
            }

            var result = await _medicineService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.MedicineID }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MedicineAPI_DTO dto)
        {
            if (dto == null || dto.MedicineID != id || dto.ResidentID == Guid.Empty)
            {
                return BadRequest();
            }

            var updated = await _medicineService.UpdateAsync(id, dto);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _medicineService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
