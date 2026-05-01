using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResidentController : Controller
    {
        private readonly IResidentDTOService _residentService;

        public ResidentController(IResidentDTOService residentService)
        {
            _residentService = residentService;
        }

        /// <summary>
        /// Gets all active residents as DTO objects.
        /// </summary>
        /// <returns>Returns all active residents.</returns>
        [HttpGet("Residents")]
        public async Task<ActionResult<IEnumerable<EditResidentDTO>>> GetAllResidentsAsync()
        {
            var residents = await _residentService.GetAllResidentsAsync();

            return Ok(residents);
        }

        /// <summary>
        /// Gets a resident by ID.
        /// </summary>
        /// <param name="id">The ID of the resident to retrieve.</param>
        /// <returns>Returns the resident if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EditResidentDTO>> GetResidentByIdAsync(Guid id)
        {
            var resident = await _residentService.GetResidentByIdAsync(id);

            if (resident == null)
            {
                return NotFound();
            }

            return Ok(resident);
        }

        /// <summary>
        /// Creates a new resident.
        /// </summary>
        /// <param name="dto">DTO object containing resident information.</param>
        /// <returns>Returns the created resident.</returns>
        [HttpPost]
        public async Task<ActionResult<EditResidentDTO>> PostResidentAsync([FromBody] EditResidentDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty)
            {
                return BadRequest();
            }

            var resident = await _residentService.PostResidentAsync(dto);
            if (resident == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetResidentByIdAsync), new { id = resident.ResidentID }, resident);
        }

        /// <summary>
        /// Updates a resident by ID.
        /// </summary>
        /// <param name="id">The ID of the resident to update.</param>
        /// <param name="dto">DTO object containing updated resident information.</param>
        /// <returns>Returns NoContent if the update succeeds, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> PutResidentAsync(Guid id, [FromBody] EditResidentDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty)
            {
                return BadRequest();
            }

            var updated = await _residentService.PutResidentAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a resident by ID.
        /// </summary>
        /// <param name="id">The ID of the resident to delete.</param>
        /// <returns>Returns NoContent if the deletion succeeds, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResidentAsync(Guid id)
        {
            var deleted = await _residentService.DeleteResidentAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Gets grocery day lookup values used when editing residents.
        /// </summary>
        /// <returns>Returns all grocery day lookup values.</returns>
        [HttpGet("groceryDays")]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetResidentGroceryDaysAsync() {
            var groceryDays = await _residentService.GetResidentGroceryDaysAsync();
            return Ok(groceryDays);
        }

        /// <summary>
        /// Gets payment method lookup values used when editing residents.
        /// </summary>
        /// <returns>Returns all payment method lookup values.</returns>
        [HttpGet("paymentMethods")]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetResidentPaymentMethodsAsync() {
            var paymentMethods = await _residentService.GetResidentPaymentMethodsAsync();
            return Ok(paymentMethods);
        }
    }
}
