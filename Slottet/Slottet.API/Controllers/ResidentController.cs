using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
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
        public async Task<ActionResult<IEnumerable<EditResidentDto>>> GetAllResidentsAsync()
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
        public async Task<ActionResult<EditResidentDto>> GetResidentByIdAsync(Guid id)
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
        public async Task<ActionResult<EditResidentDto>> PostResidentAsync([FromBody] EditResidentDto dto)
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

            return CreatedAtAction("GetResidentById", new { id = resident.ResidentID }, resident);
        }

        /// <summary>
        /// Updates a resident by ID.
        /// </summary>
        /// <param name="id">The ID of the resident to update.</param>
        /// <param name="dto">DTO object containing updated resident information.</param>
        /// <returns>Returns NoContent if the update succeeds, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> PutResidentAsync(Guid id, [FromBody] EditResidentDto dto)
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
        /// <summary>
        /// Swaps the SortOrder of two residents so they exchange positions in the grid.
        /// </summary>
        [HttpPut("swap-order")]
        public async Task<ActionResult> SwapResidentSortOrderAsync([FromBody] SwapResidentSortOrderDto dto, CancellationToken ct)
        {
            if (dto.ResidentIdA == Guid.Empty || dto.ResidentIdB == Guid.Empty || dto.ResidentIdA == dto.ResidentIdB)
                return BadRequest();

            var swapped = await _residentService.SwapResidentSortOrderAsync(dto.ResidentIdA, dto.ResidentIdB, ct);
            return swapped ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deactivates a resident (soft-delete) by setting IsActive = false.
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> DeactivateResidentAsync(Guid id, CancellationToken ct)
        {
            var deactivated = await _residentService.DeactivateResidentAsync(id, ct);
            return deactivated ? NoContent() : NotFound();
        }

        [HttpPatch("{id}/medicine-times")]
        public async Task<ActionResult> PatchMedicineTimesAsync(Guid id, [FromBody] List<TimeOnly> times, CancellationToken ct)
        {
            var updated = await _residentService.UpdateMedicineTimesAsync(id, times, ct);
            return updated ? NoContent() : NotFound();
        }

        [HttpPatch("{id}/payment-methods")]
        public async Task<ActionResult> PatchPaymentMethodsAsync(Guid id, [FromBody] List<Guid> paymentMethodIds, CancellationToken ct)
        {
            var updated = await _residentService.UpdatePaymentMethodsAsync(id, paymentMethodIds, ct);
            return updated ? NoContent() : NotFound();
        }

        [HttpPatch("{id}/grocery-day")]
        public async Task<ActionResult> PatchGroceryDayAsync(Guid id, [FromBody] Guid groceryDayId, CancellationToken ct)
        {
            if (groceryDayId == Guid.Empty)
                return BadRequest();

            var updated = await _residentService.UpdateGroceryDayAsync(id, groceryDayId, ct);
            return updated ? NoContent() : NotFound();
        }

        [HttpGet("cards")]
        public async Task<ActionResult<List<ResidentCardDto>>> GetResidentCardsAsync(CancellationToken ct)
        {
            var cards = await _residentService.GetResidentCardsAsync(ct);
            return Ok(cards);
        }

        [HttpGet("groceryDays")]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetResidentGroceryDaysAsync()
        {
            var groceryDays = await _residentService.GetResidentGroceryDaysAsync();
            return Ok(groceryDays);
        }

        /// <summary>
        /// Gets payment method lookup values used when editing residents.
        /// </summary>
        /// <returns>Returns all payment method lookup values.</returns>
        [HttpGet("paymentMethods")]
        public async Task<ActionResult<IEnumerable<ResidentLookupDTO>>> GetResidentPaymentMethodsAsync()
        {
            var paymentMethods = await _residentService.GetResidentPaymentMethodsAsync();
            return Ok(paymentMethods);
        }
    }
}
